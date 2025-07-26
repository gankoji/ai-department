# infra/main.tf

# Terraform configuration block
# Defines required providers and backend for remote state management.
# Using AWS as the default cloud provider, assuming S3 for state storage
# and DynamoDB for state locking, crucial for team collaboration and data integrity.
terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0" # Specify a compatible version
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0" # For managing K8s resources directly if needed, though Helm is preferred for apps
    }
  }

  # Backend configuration for remote state.
  # This ensures state is stored securely and is accessible by the team.
  # Replace `v7games-tf-state-unique-bucket` with a globally unique S3 bucket name.
  backend "s3" {
    bucket         = "v7games-tf-state-ancestral-guardian" # Unique bucket name for state
    key            = "terraform.tfstate"
    region         = "us-east-1" # Default region, can be overridden by var.aws_region
    encrypt        = true
    dynamodb_table = "v7games-tf-lock-ancestral-guardian" # DynamoDB table for state locking
  }
}

# Provider configuration for AWS.
# The region is dynamically set via a variable, allowing for environment-specific deployments.
provider "aws" {
  region = var.aws_region
}

# Module: Network Infrastructure
# Sets up the VPC, subnets (public/private), internet gateway, and NAT gateway.
# This forms the foundational network layer for all other services.
module "network" {
  source = "./modules/network"

  # Pass environment and project name for consistent tagging and naming.
  environment = terraform.workspace
  project_name = var.project_name

  # Network specific variables
  vpc_cidr_block = var.vpc_cidr_block
  public_subnet_cidr_blocks = var.public_subnet_cidr_blocks
  private_subnet_cidr_blocks = var.private_subnet_cidr_blocks
}

# Module: Database Infrastructure
# Provisions the database instance (e.g., RDS PostgreSQL) within the private subnets.
# This ensures the database is not publicly accessible.
module "database" {
  source = "./modules/database"

  # Pass environment and project name for consistent tagging and naming.
  environment = terraform.workspace
  project_name = var.project_name

  # Network dependencies for database placement.
  vpc_id = module.network.vpc_id
  private_subnet_ids = module.network.private_subnet_ids

  # Database specific variables.
  db_instance_type = var.db_instance_type
  db_username = var.db_username
  # db_password should be managed via a secrets manager (e.g., AWS Secrets Manager, sops)
  # For this example, it's passed as a variable, but production systems should avoid this.
  db_password = var.db_password
}

# Module: Kubernetes Cluster (EKS)
# Deploys and configures an Amazon EKS cluster, which will host our backend services.
# The cluster is placed across public and private subnets for robust access and security.
module "kubernetes_cluster" {
  source = "./modules/kubernetes"

  # Pass environment and project name for consistent tagging and naming.
  environment = terraform.workspace
  project_name = var.project_name

  # Network dependencies for EKS cluster placement.
  vpc_id = module.network.vpc_id
  private_subnet_ids = module.network.private_subnet_ids
  public_subnet_ids = module.network.public_subnet_ids # Required for ALB/NLB in EKS

  # Kubernetes cluster specific variables.
  cluster_name = var.k8s_cluster_name
}