# CeroBase - AI Agent Guide

This repository contains the **CeroBase** project, a web-based personal budget application. This `AGENTS.md` file serves as a centralized map for AI assistants working within this monorepo to ensure consistency across the different components.

## How to Use This Guide

- Start here for cross-project norms. CeroBase is a monorepo with several components.
- Each component has an `AGENTS.md` file with specific guidelines (e.g., `Fintrack.Server/AGENTS.md`, `Fintrack.Client/AGENTS.md`).
- Component docs override this file when guidance conflicts.

## 1. Project Overview
CeroBase tracks income and expenses, manages monthly budgets, and sets savings goals. It offers dashboards and reports for financial clarity, helping users control spending and make better financial decisions securely and efficiently.

## 2. Component Mapping

| Component | Location | Tech Stack |
|-----------|----------|------------|
| Frontend UI | `Fintrack.Client/` | Vue 3, TypeScript, Pinia, Vue Router, Tailwind CSS, Vite |
| Backend API | `Fintrack.Server/` | .NET 8, C#, ASP.NET Core, EF Core, PostgreSQL, MediatR |
| Unit Tests | `Fintrack.Tests/` | xUnit, NSubstitute |
| Integration Tests | `Fintrack.IntegrationTests/` | xUnit, WebApplicationFactory, Testcontainers |

## 3. Available Skills

Skills live in `.agents/skills/` and provide specialized instructions for specific tasks.

### Frontend (Vue.js)
| Skill | Description | Path |
|-------|-------------|------|
| `vue-best-practices` | Core guidelines for Vue 3 (Composition API) | [SKILL.md](.agents/skills/vue-best-practices/SKILL.md) |
| `vue-client-architecture` | Enforces 4-domain client architecture (public, auth, app, admin) | [SKILL.md](.agents/skills/vue-client-architecture/SKILL.md) |
| `vue-router-best-practices` | Routing patterns and navigation guards | [SKILL.md](.agents/skills/vue-router-best-practices/SKILL.md) |
| `vue-pinia-best-practices` | State management patterns | [SKILL.md](.agents/skills/vue-pinia-best-practices/SKILL.md) |
| `vue-testing-best-practices` | Testing with Vitest and Vue Test Utils | [SKILL.md](.agents/skills/vue-testing-best-practices/SKILL.md) |
| `vue-debug-guides` | Handling runtime errors, hydration issues, etc. | [SKILL.md](.agents/skills/vue-debug-guides/SKILL.md) |
| `vue-view-generator` | Enforces Fintrack specific layouts, ID formats, and shared components | [SKILL.md](.agents/skills/vue-view-generator/SKILL.md) |
| `create-adaptable-composable` | Standards for writing robust composables | [SKILL.md](.agents/skills/create-adaptable-composable/SKILL.md) |
| `pwa-development` | Progressive Web Apps guidelines | [SKILL.md](.agents/skills/pwa-development/SKILL.md) |

### Backend (.NET)
| Skill | Description | Path |
|-------|-------------|------|
| `backend-refactor-quality-gate` | Completion checklist for multi-layer refactors; references other skills and `Budgets` as pattern | [SKILL.md](.agents/skills/backend-refactor-quality-gate/SKILL.md) |
| `dotnet-clean-architecture` | Layer boundaries, CQRS folder layout, dependency rules | [SKILL.md](.agents/skills/dotnet-clean-architecture/SKILL.md) |
| `domain-entity-generator` | Domain Entities following DDD principles | [SKILL.md](.agents/skills/domain-entity-generator/SKILL.md) |
| `domain-events-generator` | Domain Events and MediatR handlers | [SKILL.md](.agents/skills/domain-events-generator/SKILL.md) |
| `cqrs-command-generator` | MediatR Commands and handlers | [SKILL.md](.agents/skills/cqrs-command-generator/SKILL.md) |
| `cqrs-query-generator` | MediatR Queries and handlers | [SKILL.md](.agents/skills/cqrs-query-generator/SKILL.md) |
| `fluent-validation` | Validation rules for commands/queries | [SKILL.md](.agents/skills/fluent-validation/SKILL.md) |
| `result-pattern` | Standardized error handling and result wrapping | [SKILL.md](.agents/skills/result-pattern/SKILL.md) |
| `repository-pattern` | Data access abstraction with EF Core | [SKILL.md](.agents/skills/repository-pattern/SKILL.md) |
| `specification-pattern` | Encapsulating reusable query logic | [SKILL.md](.agents/skills/specification-pattern/SKILL.md) |
| `ef-core-configuration` | EF Core entity mappings (Fluent API) | [SKILL.md](.agents/skills/ef-core-configuration/SKILL.md) |
| `supabase-postgres-best-practices` | Database optimization and queries | [SKILL.md](.agents/skills/supabase-postgres-best-practices/SKILL.md) |
| `api-controller-generator` | RESTful controller generation with MediatR | [SKILL.md](.agents/skills/api-controller-generator/SKILL.md) |
| `permission-authorization` | Granular access control using custom attributes and policies | [SKILL.md](.agents/skills/permission-authorization/SKILL.md) |
| `audit-trail` | Entity audit logging | [SKILL.md](.agents/skills/audit-trail/SKILL.md) |
| `outbox-pattern` | Reliable domain event processing | [SKILL.md](.agents/skills/outbox-pattern/SKILL.md) |
| `pipeline-behaviors` | MediatR behaviors for logging, validation, performance | [SKILL.md](.agents/skills/pipeline-behaviors/SKILL.md) |
| `quartz-background-jobs` | Scheduled background jobs | [SKILL.md](.agents/skills/quartz-background-jobs/SKILL.md) |
| `health-checks` | Application and database health checks | [SKILL.md](.agents/skills/health-checks/SKILL.md) |
| `unit-testing` | AAA pattern using xUnit and NSubstitute | [SKILL.md](.agents/skills/unit-testing/SKILL.md) |
| `integration-testing` | Real dependencies using Testcontainers | [SKILL.md](.agents/skills/integration-testing/SKILL.md) |

### General & Workflow
| Skill | Description | Path |
|-------|-------------|------|
| `brainstorming` | Define intent and requirements before creative work | [SKILL.md](.agents/skills/brainstorming/SKILL.md) |
| `conventional-commit` | Standard, descriptive commit messages | [SKILL.md](.agents/skills/conventional-commit/SKILL.md) |
| `documentation-writer` | Technical documentation (Diátaxis framework) | [SKILL.md](.agents/skills/documentation-writer/SKILL.md) |
| `excalidraw-diagram-generator` | Visualizing processes and architecture | [SKILL.md](.agents/skills/excalidraw-diagram-generator/SKILL.md) |
| `skill-creator` | Creating or updating new skills | [SKILL.md](.agents/skills/skill-creator/SKILL.md) |

## 4. Auto-invoke Skills

When performing these actions, ALWAYS invoke the corresponding skill FIRST:

| Action | Skill |
|--------|-------|
| Refactoring a domain entity or aggregate across layers | `backend-refactor-quality-gate` |
| Migrating an entity to `BaseAuditableEntityGuid` or changing PK types | `backend-refactor-quality-gate` |
| Aligning a feature with the `Budgets` pattern | `backend-refactor-quality-gate` |
| Creating or restructuring Application commands/queries | `dotnet-clean-architecture` |
| Creating a new domain entity or aggregate root | `domain-entity-generator` |
| Adding domain events to an entity | `domain-events-generator` |
| Creating a new MediatR command | `cqrs-command-generator` |
| Creating a new MediatR query | `cqrs-query-generator` |
| Adding FluentValidation to a command or query | `fluent-validation` |
| Implementing error handling with Result types | `result-pattern` |
| Creating a new repository | `repository-pattern` |
| Configuring EF Core entity mappings | `ef-core-configuration` |
| Writing or optimizing raw SQL / Postgres queries | `supabase-postgres-best-practices` |
| Creating a new API controller | `api-controller-generator` |
| Adding permission-based authorization | `permission-authorization` |
| Adding audit fields to an entity | `audit-trail` |
| Creating a background job | `quartz-background-jobs` |
| Writing unit tests | `unit-testing` |
| Writing integration tests | `integration-testing` |
| Testing repository implementations | `unit-testing` |
| Creating or modifying Vue components | `vue-best-practices` |
| Generating or restructuring a view/page UI | `vue-view-generator` |
| Adding views, routes, or layouts to the client | `vue-client-architecture` |
| Creating a composable | `create-adaptable-composable` |
| Creating a Pinia store | `vue-pinia-best-practices` |
| Writing Vue tests | `vue-testing-best-practices` |
| Debugging Vue runtime issues | `vue-debug-guides` |
| Designing a new feature (creative / ambiguous scope) | `brainstorming` |
| Creating a git commit | `conventional-commit` |
| Writing technical documentation | `documentation-writer` |
| Creating a diagram or flowchart | `excalidraw-diagram-generator` |
| Creating or updating a skill | `skill-creator` |

## 5. Development Norms
- **Skills lock (`skills-lock.json`):** If you customize a skill under `.agents/skills/`, remove that skill's entry from `skills-lock.json` when it appears there, so upstream skill installers do not overwrite local edits. See `maintenanceNote` in that file.
- **Commit Messages:** Must strictly follow the Conventional Commits specification.
- **Frontend Practices:** ALWAYS use Composition API (`<script setup>`) and TypeScript. Avoid Options API unless explicitly requested.
- **Backend Practices:** Avoid putting business logic directly in controllers; delegate to MediatR handlers and maintain type safety. When refactoring or extending backend behavior across layers, follow [`backend-refactor-quality-gate`](.agents/skills/backend-refactor-quality-gate/SKILL.md) before considering the work complete.
- **Testing Requirements:** Unit and Integration testing are STRICTLY MANDATORY for every new feature. A feature is not complete until tests are fully executed. **Crucial Routing Rule:** NEVER place tests inside the `Fintrack.Server` project. Unit tests MUST be created inside `Fintrack.Tests/` and Integration tests MUST be created inside `Fintrack.IntegrationTests/`.
