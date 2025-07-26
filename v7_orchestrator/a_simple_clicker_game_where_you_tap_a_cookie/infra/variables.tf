variable "project_name" {
  description = "A unique name for the project, used as a prefix for resources."
  type        = string
  default     = "v7games-ancestral-dough"
}

variable "environment" {
  description = "The deployment environment (e.g., 'dev', 'staging', 'prod')."
  type        = string
  default     = "dev"
}

variable "region" {
  description = "The cloud provider region to deploy resources in."
  type        = string
  default     = "us-east-1" # Assuming AWS, adjust for GCP/Azure if needed
}

# Kubernetes Cluster Variables
variable "k8s_node_count" {
  description = "The number of worker nodes for the Kubernetes cluster."
  type        = number
  default     = 2
}

variable "k8s_node_type" {
  description = "The instance type for Kubernetes worker nodes."
  type        = string
  default     = "t3.medium" # Assuming AWS EC2 instance type
}

variable "k8s_version" {
  description = "The Kubernetes version for the cluster."
  type        = string
  default     = "1.27" # Example version, check cloud provider for supported versions
}

# Database Variables (e.g., for PostgreSQL on RDS/Cloud SQL)
variable "db_instance_type" {
  description = "The instance type for the database."
  type        = string
  default     = "db.t3.micro" # Assuming AWS RDS instance type
}

variable "db_name" {
  description = "The name of the database to create."
  type        = string
  default     = "ancestraldoughdb"
}

variable "db_username" {
  description = "The master username for the database."
  type        = string
  default     = "doughmaster"
}

variable "db_password" {
  description = "The master password for the database. IMPORTANT: For production, use a secrets manager (e.g., AWS Secrets Manager, Vault) instead of hardcoding or passing directly."
  type        = string
  sensitive   = true
}

# Network Variables (if not using default VPC/VNet)
variable "vpc_cidr_block" {
  description = "The CIDR block for the VPC."
  type        = string
  default     = "10.0.0.0/16"
}

variable "public_subnet_cidrs" {
  description = "A list of CIDR blocks for public subnets."
  type        = list(string)
  default     = ["10.0.1.0/24", "10.0.2.0/24"]
}

variable "private_subnet_cidrs" {
  description = "A list of CIDR blocks for private subnets."
  type        = list(string)
  default     = ["10.0.10.0/24", "10.0.11.0/24"]
}