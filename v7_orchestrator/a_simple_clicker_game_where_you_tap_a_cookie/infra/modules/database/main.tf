resource "aws_db_subnet_group" "main" {
  name       = "${var.environment}-db-subnet-group"
  subnet_ids = var.private_subnet_ids

  tags = {
    Name        = "${var.environment}-db-subnet-group"
    Environment = var.environment
    Project     = "V7Games"
  }
}

resource "aws_security_group" "db_sg" {
  name_prefix = "${var.environment}-db-sg-"
  description = "Security group for V7 Games backend database in ${var.environment} environment"
  vpc_id      = var.vpc_id

  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    # This should ideally be restricted to the backend service's security group
    # For simplicity, allowing from VPC CIDR, but for production, narrow this down.
    cidr_blocks = [var.vpc_cidr_block] 
    description = "Allow PostgreSQL traffic from within the VPC"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
    description = "Allow all outbound traffic"
  }

  tags = {
    Name        = "${var.environment}-db-sg"
    Environment = var.environment
    Project     = "V7Games"
  }
}

resource "aws_db_instance" "main" {
  allocated_storage    = var.db_allocated_storage
  engine               = "postgres"
  engine_version       = "15.3" # Or a suitable current version
  instance_class       = var.db_instance_class
  db_name              = var.db_name
  username             = var.db_username
  password             = var.db_password
  parameter_group_name = "default.postgres15"
  skip_final_snapshot  = true # Set to false for production
  publicly_accessible  = false
  multi_az             = false # Set to true for production high availability

  db_subnet_group_name   = aws_db_subnet_group.main.name
  vpc_security_group_ids = [aws_security_group.db_sg.id]

  # For production, consider enabling deletion protection
  # deletion_protection = true

  tags = {
    Name        = "${var.environment}-v7games-db"
    Environment = var.environment
    Project     = "V7Games"
    Service     = "BackendDatabase"
  }
}

output "db_instance_address" {
  description = "The address of the RDS instance"
  value       = aws_db_instance.main.address
}

output "db_instance_port" {
  description = "The port of the RDS instance"
  value       = aws_db_instance.main.port
}

output "db_security_group_id" {
  description = "The ID of the security group attached to the database instance"
  value       = aws_security_group.db_sg.id
}

variable "environment" {
  description = "The deployment environment (e.g., dev, staging, prod)"
  type        = string
}

variable "vpc_id" {
  description = "The ID of the VPC where the database will be deployed"
  type        = string
}

variable "vpc_cidr_block" {
  description = "The CIDR block of the VPC"
  type        = string
}

variable "private_subnet_ids" {
  description = "A list of private subnet IDs for the database subnet group"
  type        = list(string)
}

variable "db_name" {
  description = "The name of the database"
  type        = string
  default     = "v7gamesdb"
}

variable "db_username" {
  description = "The master username for the database"
  type        = string
}

variable "db_password" {
  description = "The master password for the database"
  type        = string
  sensitive   = true
}

variable "db_instance_class" {
  description = "The instance type of the RDS database"
  type        = string
  default     = "db.t3.micro" # Smallest for dev, scale up for prod
}

variable "db_allocated_storage" {
  description = "The allocated storage in GB for the database"
  type        = number
  default     = 20
}