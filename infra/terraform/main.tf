# --- Terraform root module (placeholder) -----------------
terraform {
  required_version = ">= 1.7.0"
  required_providers {
    kubernetes = { source = "hashicorp/kubernetes" version = ">= 2.24.0" }
    helm       = { source = "hashicorp/helm"       version = ">= 2.13.0" }
    aws        = { source = "hashicorp/aws"        version = ">= 5.39.0" }
  }
}

# TODO: remote backend, provider credentials

module "postgres" {
  source = "./modules/postgres"   # to be implemented
}

module "vault" {
  source = "./modules/vault"      # to be implemented
}

module "object_store" {
  source = "./modules/s3"         # to be implemented
}
