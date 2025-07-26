output "kubernetes_cluster_name" {
  description = "The name of the Kubernetes cluster."
  value       = module.kubernetes.cluster_name
}

output "kubernetes_cluster_endpoint" {
  description = "The endpoint of the Kubernetes cluster."
  value       = module.kubernetes.cluster_endpoint
}

output "kubernetes_cluster_ca_certificate" {
  description = "The base64 encoded CA certificate of the Kubernetes cluster."
  value       = module.kubernetes.cluster_ca_certificate
}

output "database_endpoint" {
  description = "The connection endpoint for the primary database instance."
  value       = module.database.db_endpoint
  sensitive   = true
}

output "database_name" {
  description = "The name of the database instance."
  value       = module.database.db_name
}

output "database_username" {
  description = "The master username for the database."
  value       = module.database.db_username
  sensitive   = true
}

output "vpc_id" {
  description = "The ID of the created Virtual Private Cloud (VPC)."
  value       = module.network.vpc_id
}

output "public_subnet_ids" {
  description = "List of IDs of the public subnets."
  value       = module.network.public_subnet_ids
}

output "private_subnet_ids" {
  description = "List of IDs of the private subnets."
  value       = module.network.private_subnet_ids
}