# V7 Orchestrator – Implementation Design (v1)

This file merges the detailed design, critique responses, and the final operational / infrastructure decisions confirmed on 27 Jul 2025.

## 1 Service-Level Goals
| Metric | Target | Rationale |
|--------|--------|-----------|
| Availability | ≥ 95 % | Favors simplicity / cost; single-AZ acceptable |
| Max concurrent projects | 5 | v1 scope |
| Max agents | 50 | v1 scope |
| Monthly cost ceiling | 100 USD | Early-stage budget guard-rail |

## 2 Architecture Snapshot
…existing content from `detailed-design.md` (diagram, modules, data model)…

## 3 Additional Decisions After Gap Analysis
| Topic | Choice | Notes |
|-------|--------|-------|
| Queue tech | **PostgreSQL LISTEN/NOTIFY** | Avoid extra infra; revisit at >50 agents |
| Object storage | Shared S3-compatible bucket | Per-env prefixes (`dev/`, `prod/`) keep data siloed |
| Secrets | Vault, single cluster | Rotated monthly; bootstrap via Terraform |
| Monitoring | Prometheus + Grafana + Loki (Helm) | PagerDuty & Slack alerts |
| CI/CD | GitHub Actions → GHCR → Helm upgrade | No image-signing yet, but workflow structured for future cosign |
| API evolution | Versioned (`/api/v1`) endpoints | Leave room for email approval (v2) |

## 4 Infrastructure Components

| Layer | Tool | File(s) |
|-------|------|---------|
| Kubernetes base | Helm chart | `infra/helm/*` |
| PostgreSQL (single AZ) | Terraform module | `infra/terraform/postgres.tf` |
| Vault | Terraform module | `infra/terraform/vault.tf` |
| Telemetry stack | Helm dependencies | `infra/helm/values.yaml` |
| Object store bucket | Terraform | `infra/terraform/s3.tf` |

## 5 Operations Runbook (Condensed)
1. **Create project** – push branch → GitHub Action → `/api/v1/projects`  
2. **Daily 03:00 UTC cron** – DB dump to `s3://ops-backup/dev|prod/…`  
3. **Alerts** –  
   • `scheduler_lag_seconds > 300`  
   • PG disk > 80 %  
   • Token budget ≥ 95 %  
   • Helm release in failed state  
4. **Monthly** – rotate Vault root token & run restore drill.

## 6 Open TODOs
- Complete Terraform variables / outputs
- Flesh out Helm template values
- Implement CLI bootstrap script `make dev`
- Write synthetic E2E test harness
