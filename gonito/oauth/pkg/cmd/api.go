package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"strconv"
	"strings"

	"github.com/go-chi/chi/v5"
	"github.com/lestrrat-go/jwx/jwt"

	"github.com/go-chi/chi/v5/middleware"

	"golang.org/x/oauth2"
)

func main() {

	router := chi.NewRouter()
	router.Use(middleware.Logger)

	router.Get("/", func(w http.ResponseWriter, r *http.Request) {
		_, err := w.Write([]byte("Welcome to the oauth2 service."))
		if err != nil {
			return
		}
	})

	// Oauth
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
	var p []string
	t, err := exchangeCodeForToken(ctx, r)
	if err != nil {
		log.Fatal(err)
		return
	}
	jt, err := getJWT(err, t)
	if err != nil {
		log.Fatal(err)
		return
	}

	res := parseToken(r, t, jt, p)
	_, err = w.Write([]byte(res))
	if err != nil {
		log.Fatal(err)
		return
	}
}

func parseToken(r *http.Request, t *oauth2.Token, jt jwt.Token, p []string) string {
	c := fmt.Sprintln("Code: ", r.FormValue("code"))
	v := fmt.Sprintln("Token is valid: ", strconv.FormatBool(t.Valid()))
	at := fmt.Sprintln("Access token: ", t.AccessToken)
	it := fmt.Sprintln("ID token: ", t.Extra("id_token").(string))
	iss := fmt.Sprintln("Issuer: ", jt.Issuer())
	aud := fmt.Sprintln("Audience: ", jt.Audience()[0])
	p = []string{c, v, at, it, iss, aud}
	un, exists := jt.Get("cognito:username")
	if exists == true {
		p = append(p, fmt.Sprintln("Username: ", un.(string)))
	}
	d, exists := jt.Get("custom:department")
	if exists == true {
		p = append(p, fmt.Sprintln("Department: ", d.(string)))
	}

	res := strings.Join(p, "\n")
	return res
}

func getJWT(err error, t *oauth2.Token) (jwt.Token, error) {
	jt, err := jwt.Parse([]byte(t.Extra("id_token").(string)))
	return jt, err
}

func exchangeCodeForToken(ctx context.Context, r *http.Request) (*oauth2.Token, error) {
	t, err := conf.Exchange(ctx, r.FormValue("code"))
	return t, err
}
