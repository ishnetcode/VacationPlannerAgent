resource "aws_ecr_repository" "app" {
  name         = "vacation-planner"
  force_delete = true # lets 'terraform destroy' remove it even if it still contains images
}