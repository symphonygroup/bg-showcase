package auth

import (
	"fmt"
	"net/http"
	"os"
	"strings"

	"github.com/lestrrat-go/jwx/jwk"
	"github.com/lestrrat-go/jwx/jwt"
)

func CheckClaims(writer http.ResponseWriter, token jwt.Token) (hasError bool) {
	username, _ := token.Get("cognito:username")
	department, _ := token.Get("custom:department")
	auds, _ := token.Get("aud")
	aud, ok := auds.([]string)

	if !ok && len(aud) != 1 {
		fmt.Printf("Missing audience from JWT.\n")
		http.Error(writer, http.StatusText(http.StatusUnauthorized), http.StatusUnauthorized)
		return true
	}

	fmt.Printf("Username: %v, Department: %v, Audience: %v\n", username, department, auds)

	if auds.([]string)[0] != os.Getenv("AUD") {
		fmt.Printf("Invalid audience %v. Expected audience %v\n", aud[0], os.Getenv("AUD"))
		http.Error(writer, http.StatusText(http.StatusUnauthorized), http.StatusUnauthorized)
		return true
	}

	return false
}

func FetchJWK(writer http.ResponseWriter, request *http.Request, cognitoClient *CognitoClient) (jwk.Set, bool) {
	pubKeyURL := "https://cognito-idp.%s.amazonaws.com/%s/.well-known/jwks.json"
	formattedURL := fmt.Sprintf(pubKeyURL, os.Getenv("AWS_DEFAULT_REGION"), cognitoClient.UserPoolId)
	keySet, err := jwk.Fetch(request.Context(), formattedURL)

	if err != nil {
		http.Error(writer, err.Error(), http.StatusInternalServerError)
		return nil, true
	}

	return keySet, false
}

func GetCognitoClient(writer http.ResponseWriter, r *http.Request) (*CognitoClient, bool) {
	cognitoClient, ok := r.Context().Value("CognitoClient").(*CognitoClient)

	if !ok {
		http.Error(writer, "Could not retrieve CognitoClient from context.", http.StatusInternalServerError)
		return nil, true
	}

	return cognitoClient, false
}

func SplitAuthHeader(writer http.ResponseWriter, authHeader string) (splitAuthHeader []string, hasError bool) {
	splitAuthHeader = strings.Split(authHeader, " ")

	if len(splitAuthHeader) != 2 {
		http.Error(writer, "Missing or invalid authorization header.", http.StatusUnauthorized)
		return nil, true
	}

	return splitAuthHeader, false
}

func ParseJWT(writer http.ResponseWriter, splitAuthHeader []string, keySet jwk.Set) (jwt.Token, bool) {

	token, err := jwt.Parse(
		[]byte(splitAuthHeader[1]),
		jwt.WithKeySet(keySet),
		jwt.WithValidate(true),
	)

	if err != nil {
		fmt.Println("Invalid or expired JWT.")
		http.Error(writer, err.Error(), http.StatusBadRequest)
		return nil, true
	}

	return token, false
}

func ValidateClaims(writer http.ResponseWriter, request *http.Request) (hasErr bool) {
	authHeader := request.Header.Get("Authorization")

	splitAuthHeader, hasErr := SplitAuthHeader(writer, authHeader)
	if hasErr {
		return true
	}

	cognitoClient, hasErr := GetCognitoClient(writer, request)
	if hasErr {
		return true
	}

	keySet, hasErr := FetchJWK(writer, request, cognitoClient)
	if hasErr {
		return true
	}

	token, hasErr := ParseJWT(writer, splitAuthHeader, keySet)
	if hasErr {
		return true
	}

	if CheckClaims(writer, token) {
		return true
	}

	return false
}
