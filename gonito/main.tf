// Variables
variable "aws_region" {
  type        = string
  description = "The region in which the resources will be created"
  default     = "eu-central-1"
}
terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~>4.30.0"
    }
  }
}

provider "aws" {
  region = "eu-central-1"
}

// Resources
resource "aws_cognito_user_pool" "oauth_demo" {
  name = "oauth_demo"

  username_attributes      = ["email"]
  auto_verified_attributes = ["email"]
  password_policy {
    minimum_length = 6
  }

  schema {
    attribute_data_type      = "String"
    developer_only_attribute = false
    mutable                  = true
    name                     = "department"
    required                 = false
    string_attribute_constraints {
      min_length = 1
      max_length = 2048
    }
  }
}

resource "aws_cognito_user_pool_domain" "main" {
  domain       = "oa2"
  user_pool_id = aws_cognito_user_pool.oauth_demo.id
}

resource "aws_cognito_user_pool_client" "client1" {
  name = "client1"

  user_pool_id                  = aws_cognito_user_pool.oauth_demo.id
  generate_secret               = true
  refresh_token_validity        = 90
  prevent_user_existence_errors = "ENABLED"
  explicit_auth_flows = [
    "ALLOW_REFRESH_TOKEN_AUTH",
    "ALLOW_USER_PASSWORD_AUTH",
    "ALLOW_ADMIN_USER_PASSWORD_AUTH",
    "ALLOW_CUSTOM_AUTH",
    "ALLOW_USER_SRP_AUTH"
  ]
  callback_urls                        = ["http://localhost:8080/auth/callback"]
  allowed_oauth_flows_user_pool_client = true
  allowed_oauth_flows                  = ["code"]
  allowed_oauth_scopes                 = ["aws.cognito.signin.user.admin", "email", "openid"]
  supported_identity_providers         = ["COGNITO"]
}