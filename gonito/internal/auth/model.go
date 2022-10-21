package auth

import "github.com/aws/aws-sdk-go-v2/service/cognitoidentityprovider/types"

type SignUpRequest struct {
	Username       string `json:"username"`
	Password       string `json:"password"`
	Aud            string `json:"aud"`
	UserAttributes []types.AttributeType
}

type SignInRequest struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

type SignInResponse struct {
	AccessToken  *string `json:"access_token"`
	ExpiresIn    int32   `json:"expires_in"`
	IdToken      *string `json:"id_token"`
	RefreshToken *string `json:"refresh_token"`
	TokenType    *string `json:"token_type"`
}
