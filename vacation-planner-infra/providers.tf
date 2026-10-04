terraform {
  required_version = ">= 1.6"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
  }
}

provider "aws" {
  region  = "eu-west-1"
  profile = "ish-admin" # your SSO profile - Terraform understands SSO logins

  # Tags added automatically to EVERY resource Terraform creates
  default_tags {
    tags = {
      Project   = "vacation-planner"
      ManagedBy = "terraform"
    }
  }
}