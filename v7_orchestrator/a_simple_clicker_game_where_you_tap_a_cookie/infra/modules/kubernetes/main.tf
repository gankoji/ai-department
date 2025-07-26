# infra/modules/kubernetes/main.tf

# This module provisions an AWS EKS Kubernetes cluster and its associated resources.

# EKS Cluster IAM Role
resource "aws_iam_role" "eks_cluster_role" {
  name = "${var.project_name}-eks-cluster-role-${var.environment}"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "eks.amazonaws.com"
        }
        Action = "sts:AssumeRole"
      }
    ]
  })

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

resource "aws_iam_role_policy_attachment" "eks_cluster_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSClusterPolicy"
  role       = aws_iam_role.eks_cluster_role.name
}

resource "aws_iam_role_policy_attachment" "eks_vpc_resource_controller_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSVPCResourceController"
  role       = aws_iam_role.eks_cluster_role.name
}

# EKS Cluster
resource "aws_eks_cluster" "main_cluster" {
  name     = "${var.project_name}-cluster-${var.environment}"
  role_arn = aws_iam_role.eks_cluster_role.arn
  version  = var.kubernetes_version

  vpc_config {
    subnet_ids             = var.private_subnet_ids
    security_group_ids     = [aws_security_group.eks_cluster_sg.id]
    endpoint_private_access = true # Allow internal access to API server
    endpoint_public_access  = false # Disable public access to API server, good for security, requires bastion/VPN
  }

  enabled_cluster_log_types = ["api", "audit", "authenticator", "controllerManager", "scheduler"]

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }

  # Ensure that IAM Role for EKS Cluster is created before EKS Cluster
  depends_on = [
    aws_iam_role_policy_attachment.eks_cluster_policy,
    aws_iam_role_policy_attachment.eks_vpc_resource_controller_policy,
  ]
}

# EKS Node Group IAM Role
resource "aws_iam_role" "eks_node_role" {
  name = "${var.project_name}-eks-node-role-${var.environment}"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "ec2.amazonaws.com"
        }
        Action = "sts:AssumeRole"
      }
    ]
  })

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

resource "aws_iam_role_policy_attachment" "eks_worker_node_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKSWorkerNodePolicy"
  role       = aws_iam_role.eks_node_role.name
}

resource "aws_iam_role_policy_attachment" "eks_cni_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEKS_CNI_Policy"
  role       = aws_iam_role.eks_node_role.name
}

resource "aws_iam_role_policy_attachment" "ec2_container_registry_read_only_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
  role       = aws_iam_role.eks_node_role.name
}

# EKS Node Group
resource "aws_eks_node_group" "main_node_group" {
  cluster_name    = aws_eks_cluster.main_cluster.name
  node_group_name = "${var.project_name}-node-group-${var.environment}"
  node_role_arn   = aws_iam_role.eks_node_role.arn
  subnet_ids      = var.private_subnet_ids
  instance_types  = [var.instance_type]
  disk_size       = var.node_disk_size_gb

  scaling_config {
    desired_size = var.node_group_desired_capacity
    min_size     = var.node_group_min_capacity
    max_size     = var.node_group_max_capacity
  }

  update_config {
    max_unavailable = 1
  }

  # Ensure that IAM Role for Node Group is created before EKS Node Group
  depends_on = [
    aws_iam_role_policy_attachment.eks_worker_node_policy,
    aws_iam_role_policy_attachment.eks_cni_policy,
    aws_iam_role_policy_attachment.ec2_container_registry_read_only_policy,
  ]

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

# Security Group for EKS Cluster
resource "aws_security_group" "eks_cluster_sg" {
  name        = "${var.project_name}-eks-cluster-sg-${var.environment}"
  description = "Security group for EKS cluster control plane"
  vpc_id      = var.vpc_id

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

# Add ingress rule to allow nodes to communicate with the control plane
resource "aws_security_group_rule" "eks_node_to_cluster_ingress" {
  type              = "ingress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  source_security_group_id = aws_security_group.eks_node_sg.id # Assuming node SG will be created in this module or passed in
  security_group_id = aws_security_group.eks_cluster_sg.id
  description       = "Allow EKS nodes to communicate with the cluster control plane"
}

# Security Group for EKS Nodes
resource "aws_security_group" "eks_node_sg" {
  name        = "${var.project_name}-eks-node-sg-${var.environment}"
  description = "Security group for EKS worker nodes"
  vpc_id      = var.vpc_id

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"] # This should be restricted based on actual needs, e.g., ALB/NLB SG
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

# Add ingress rule to allow cluster to communicate with the nodes
resource "aws_security_group_rule" "eks_cluster_to_node_ingress" {
  type              = "ingress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  source_security_group_id = aws_security_group.eks_cluster_sg.id
  security_group_id = aws_security_group.eks_node_sg.id
  description       = "Allow EKS cluster control plane to communicate with nodes"
}

# Output the cluster details for kubeconfig generation or other modules
output "cluster_name" {
  description = "The name of the EKS cluster."
  value       = aws_eks_cluster.main_cluster.name
}

output "cluster_endpoint" {
  description = "The endpoint for the EKS cluster."
  value       = aws_eks_cluster.main_cluster.endpoint
}

output "cluster_certificate_authority_data" {
  description = "The certificate authority data for the EKS cluster."
  value       = aws_eks_cluster.main_cluster.certificate_authority[0].data
}

output "cluster_id" {
  description = "The ID of the EKS cluster."
  value       = aws_eks_cluster.main_cluster.id
}

output "eks_node_group_arn" {
  description = "ARN of the EKS Node Group."
  value       = aws_eks_node_group.main_node_group.arn
}

output "eks_node_role_arn" {
  description = "ARN of the EKS Node IAM Role."
  value       = aws_iam_role.eks_node_role.arn
}