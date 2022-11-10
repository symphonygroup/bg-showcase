package main

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"

	"github.com/coreos/go-oidc/v3/oidc"
	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"golang.org/x/oauth2"
)

var claims struct {
	Verified   bool   `json:"email_verified"`
	Department string `json:"custom:department"`
	Issuer     string `json:"iss"`
	Username   string `json:"cognito:username"`
	Audience   string `json:"aud"`
	Exp        int64  `json:"exp"`
}

func main() {

	router := chi.NewRouter()
	router.Use(middleware.Logger)

	router.Get("/", func(w http.ResponseWriter, r *http.Request) {
		_, err := w.Write([]byte("Welcome to the oauth2 service."))
		if err != nil {
			return
		}
	})

	router.Get("/auth/login", oauthLogin)
	router.Get("/auth/callback", oauthCallback)

	port := os.Getenv("PORT")
	fmt.Println("Starting the oauth service.")
	err := http.ListenAndServe(fmt.Sprintf(":%s", port), router)
	if err != nil {
		return
	}
}

var conf = &oauth2.Config{
	ClientID:     os.Getenv("COGNITO_APP_CLIENT_ID"),
	ClientSecret: os.Getenv("COGNITO_OAUTH_CLIENT_SECRET"),
	RedirectURL:  os.Getenv("REDIRECT_URL"),
	// Scopes:       []string{"scope_1", "scope_2"},
	Endpoint: oauth2.Endpoint{
		AuthURL:  os.Getenv("AUTH_URL"),
		TokenURL: os.Getenv("TOKEN_URL"),
	},
}

func oauthLogin(w http.ResponseWriter, r *http.Request) {
	url := conf.AuthCodeURL("state", oauth2.AccessTypeOffline)
	http.Redirect(w, r, url, http.StatusTemporaryRedirect)
}

func oauthCallback(w http.ResponseWriter, r *http.Request) {
	ctx := context.Background()
	t, err := exchangeCodeForToken(ctx, r)
	if err != nil {
		log.Fatal(err)
		return
	}

	getClaims(err, ctx, t)
	c, err := json.MarshalIndent(claims, "", "\t")
	if err != nil {
		log.Fatal(err)
		return
	}

	_, err = w.Write(c)
	if err != nil {
		log.Fatal(err)
		return
	}
}

func getClaims(err error, ctx context.Context, t *oauth2.Token) {
	provider, err := oidc.NewProvider(ctx, os.Getenv("ISSUER"))
	if err != nil {
		log.Fatal(err)
	}
	oidcConfig := &oidc.Config{
		ClientID: os.Getenv("COGNITO_APP_CLIENT_ID"),
	}
	verifier := provider.Verifier(oidcConfig)

	rawIDToken, ok := t.Extra("id_token").(string)
	if !ok {
		log.Fatal("Missing ID Token.")
		return
	}

	idToken, err := verifier.Verify(ctx, rawIDToken)
	if err != nil {
		log.Fatal(err)
		return
	}

	if err := idToken.Claims(&claims); err != nil {
		log.Fatal(err)
		return
	}
	fmt.Println(claims)
}

func exchangeCodeForToken(ctx context.Context, r *http.Request) (*oauth2.Token, error) {
	t, err := conf.Exchange(ctx, r.FormValue("code"))
	return t, err
}
