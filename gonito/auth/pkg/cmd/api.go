package main

import (
	"encoding/json"
	"fmt"
	"net/http"
	"os"

	"github.com/go-chi/chi/v5"

	"github.com/milennik/gonito/internal/auth"

	"github.com/aws/aws-sdk-go-v2/aws"
	cip "github.com/aws/aws-sdk-go-v2/service/cognitoidentityprovider"
	"github.com/go-chi/chi/v5/middleware"
)

func main() {
	cognitoClient := auth.Init()

	router := chi.NewRouter()
	router.Use(middleware.Logger, middleware.WithValue("CognitoClient", cognitoClient))

	// Public Endpoints
	router.Get("/", func(w http.ResponseWriter, r *http.Request) {
		_, err := w.Write([]byte("Welcome to the auth service."))
		if err != nil {
			return
		}
	})
	router.Post("/signin", signIn)
	router.Post("/signup", signUp)
	// Private Endpoints
	router.Group(func(r chi.Router) {
		r.Use(IsAuth)
		r.Get("/test", testAuth)
	})

	port := os.Getenv("PORT")
	fmt.Println("Starting auth service.")
	err := http.ListenAndServe(fmt.Sprintf(":%s", port), router)
	if err != nil {
		return
	}
}

func IsAuth(next http.Handler) http.Handler {
	return http.HandlerFunc(func(writer http.ResponseWriter, request *http.Request) {
		if auth.ValidateClaims(writer, request) {
			return
		}
		// Token is authenticated, pass it through
		next.ServeHTTP(writer, request)
	})
}

func signUp(writer http.ResponseWriter, request *http.Request) {

	var req auth.SignUpRequest
	err := json.NewDecoder(request.Body).Decode(&req)
	if err != nil {
		http.Error(writer, err.Error(), http.StatusBadRequest)
		return
	}

	cognitoClient, hasErr := auth.GetCognitoClient(writer, request)
	if hasErr {
		return
	}

	awsReq := &cip.SignUpInput{
		ClientId:       aws.String(cognitoClient.AppClientId),
		Username:       aws.String(req.Username),
		Password:       aws.String(req.Password),
		UserAttributes: req.UserAttributes,
	}

	cognitoClient.AppClientId = req.Aud
	_, err = cognitoClient.SignUp(request.Context(), awsReq)
	if err != nil {
		http.Error(writer, err.Error(), http.StatusInternalServerError)
		return
	}

	confirmInput := &cip.AdminConfirmSignUpInput{
		UserPoolId: aws.String(cognitoClient.UserPoolId),
		Username:   aws.String(req.Username),
	}

	_, err = cognitoClient.AdminConfirmSignUp(request.Context(), confirmInput)
	if err != nil {
		http.Error(writer, err.Error(), http.StatusInternalServerError)
		return
	}

	_, err = writer.Write([]byte("Successful sign up."))
	if err != nil {
		return
	}
}

func signIn(writer http.ResponseWriter, request *http.Request) {

	var req auth.SignInRequest
	err := json.NewDecoder(request.Body).Decode(&req)
	if err != nil {
		http.Error(writer, err.Error(), http.StatusBadRequest)
		return
	}

	cognitoClient, hasErr := auth.GetCognitoClient(writer, request)
	if hasErr {
		return
	}

	signInInput := &cip.AdminInitiateAuthInput{
		AuthFlow:       "ADMIN_USER_PASSWORD_AUTH",
		ClientId:       aws.String(cognitoClient.AppClientId),
		UserPoolId:     aws.String(cognitoClient.UserPoolId),
		AuthParameters: map[string]string{"USERNAME": req.Username, "PASSWORD": req.Password},
	}

	output, err := cognitoClient.AdminInitiateAuth(request.Context(), signInInput)
	if err != nil {
		http.Error(writer, err.Error(), http.StatusBadRequest)
		return
	}

	res := &auth.SignInResponse{
		AccessToken:  output.AuthenticationResult.AccessToken,
		ExpiresIn:    output.AuthenticationResult.ExpiresIn,
		IdToken:      output.AuthenticationResult.IdToken,
		RefreshToken: output.AuthenticationResult.RefreshToken,
		TokenType:    output.AuthenticationResult.TokenType,
	}
	_ = json.NewEncoder(writer).Encode(res)
}

func testAuth(writer http.ResponseWriter, request *http.Request) {
	_, _ = writer.Write([]byte("Hello from the test endpoint, JWT is valid.\n"))
	return
}
