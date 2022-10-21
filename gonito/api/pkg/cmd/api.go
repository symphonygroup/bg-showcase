package main

import (
	"fmt"
	"net/http"
	"os"

	"github.com/go-chi/chi/v5"

	"github.com/milennik/gonito/internal/auth"

	"github.com/go-chi/chi/v5/middleware"
)

func main() {
	cognitoClient := auth.Init()

	router := chi.NewRouter()
	router.Use(middleware.Logger, middleware.WithValue("CognitoClient", cognitoClient))

	// Public Endpoints
	router.Get("/", func(w http.ResponseWriter, r *http.Request) {
		_, err := w.Write([]byte("Welcome to the test API."))
		if err != nil {
			return
		}
	})

	// Private Endpoints
	router.Group(func(r chi.Router) {
		r.Use(IsAuth)
		r.Get("/test", testAuth)
	})

	port := os.Getenv("PORT")

	fmt.Println("Starting the test API.")
	err := http.ListenAndServe(fmt.Sprintf(":%s", port), router)
	if err != nil {
		return
	}
}

func testAuth(writer http.ResponseWriter, request *http.Request) {
	_, _ = writer.Write([]byte("Hello from the test endpoint, JWT is valid.\n"))
	return
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
