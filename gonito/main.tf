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
resource "aws_cognito_user_pool" "terra_user_pool" {
  name = "terra_user_pool"

  #  username_attributes      = ["email"]
  #  auto_verified_attributes = ["email"]
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

  schema {
    attribute_data_type      = "String"
    developer_only_attribute = false
    mutable                  = true
    name                     = "new_custom_attr"
    required                 = false
    string_attribute_constraints {
      min_length = 1
      max_length = 2048
    }
  }
}

resource "aws_cognito_user_pool_client" "client" {
  name = "terra_cognito_client"

  user_pool_id                  = aws_cognito_user_pool.terra_user_pool.id
  generate_secret               = false
  refresh_token_validity        = 90
  prevent_user_existence_errors = "ENABLED"
  explicit_auth_flows = [
    "ALLOW_REFRESH_TOKEN_AUTH",
    "ALLOW_USER_PASSWORD_AUTH",
    "ALLOW_ADMIN_USER_PASSWORD_AUTH",
    "ALLOW_CUSTOM_AUTH",
    "ALLOW_USER_SRP_AUTH"
  ]
}

resource "aws_cognito_user_pool_client" "terra_cognito_client2" {
  name = "terra_cognito_client2"

  user_pool_id                  = aws_cognito_user_pool.terra_user_pool.id
  generate_secret               = false
  refresh_token_validity        = 90
  prevent_user_existence_errors = "ENABLED"
  explicit_auth_flows = [
    "ALLOW_REFRESH_TOKEN_AUTH",
    "ALLOW_USER_PASSWORD_AUTH",
    "ALLOW_ADMIN_USER_PASSWORD_AUTH",
    "ALLOW_CUSTOM_AUTH",
    "ALLOW_USER_SRP_AUTH"
  ]
}