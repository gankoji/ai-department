# V7 Orchestrator – Design Critiques (27 Jul 2025)

This document records the main failure-mode analysis of `fsm_orchestrator/detailed-design.md`.  
Items are ranked by potential business impact.

| # | Issue | Risk | Root Cause | Initial Mitigation Ideas |
|---|-------|------|------------|--------------------------|
| 1 | State-explosion & maintainability | High | 1:1 agent↔state mapping will not scale | Switch to hierarchical FSM, strict naming conventions, code-gen docs |
| 2 | **Memory compression & context loss** | High | Embedding summaries may omit critical nuance | Dual-layer memory, RAG retrieval, “important-decision” tagging |
| 3 | **Lack of agent scheduling** | High | No prioritisation → starvation / overload | Dedicated task queue + cost-aware scheduler |
| 4 | Orchestrator is a SPOF | Medium | Single monolith, no HA | Replicas + leader election or micro-services |
| 5 | Cost controls too coarse | Medium | Per-project only | Per-task budget caps and alerts |
| 6 | Observability gaps | Medium | Undefined KPIs | Define metrics & alert thresholds up-front |
| 7 | Security / RBAC thin | Low (small team) | OAuth only | Add roles + audit trail |
| 8 | DB could bottleneck | Low | All data in single PG | Table partitioning, read replicas, vector store off-load |