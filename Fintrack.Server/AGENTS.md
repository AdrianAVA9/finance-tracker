# CeroBase Server - AI Agent Guide

This document provides specialized context and rules for AI agents working within the `Fintrack.Server` directory. It overrides general knowledge and ensures code aligns with the established backend architecture and standards.

## How to Use This Guide

- This file is the **authoritative** reference for all backend work in `Fintrack.Server/`.
- For cross-project norms, see the root [`AGENTS.md`](../AGENTS.md).
- Skills live in `../.agents/skills/`. Always read a skill file before applying it.
- The in-repo `Budgets` feature (`Domain/Budgets/`, `Application/Budgets/`, etc.) is the **gold-standard pattern**. Match its structure when implementing or refactoring any feature.

## 1. Context & Tech Stack
| Item | Details |
|------|---------|
| Framework | .NET 8, ASP.NET Core |
| Language | C# |
| Database ORM | Entity Framework Core (EF Core) |
| Database Provider | PostgreSQL (Npgsql) |
| Architecture | Clean Architecture, CQRS (via MediatR) |
| Testing | xUnit, NSubstitute, WebApplicationFactory, Testcontainers |

## 2. Architecture: Clean Architecture
The backend adheres strictly to Clean Architecture principles. Agents MUST respect these boundaries:
- **`Domain/`**: Enterprise logic and entities. No dependencies on outer layers.
- **`Application/`**: Business logic, use cases (CQRS handlers), and DTOs. Depends only on `Domain`.
- **`Infrastructure/`**: Data access, external services, and identity implementations.
- **`Api/`** (or `Controllers/`): The presentation layer. HTTP endpoints, middleware, and DI setup.

## 3. Mandatory Development Workflow

Every new backend feature MUST be implemented following these numbered steps (0-6). Do not skip steps. Testing (Step 6) is strictly mandatory.

| Step | Layer | Skills |
|------|-------|--------|
| 0 | Structure & refactor gates | [`backend-refactor-quality-gate`](../.agents/skills/backend-refactor-quality-gate/SKILL.md), [`dotnet-clean-architecture`](../.agents/skills/dotnet-clean-architecture/SKILL.md) |
| 1 | Domain | [`domain-entity-generator`](../.agents/skills/domain-entity-generator/SKILL.md), [`domain-events-generator`](../.agents/skills/domain-events-generator/SKILL.md) |
| 2 | Application | [`cqrs-command-generator`](../.agents/skills/cqrs-command-generator/SKILL.md), [`cqrs-query-generator`](../.agents/skills/cqrs-query-generator/SKILL.md), [`fluent-validation`](../.agents/skills/fluent-validation/SKILL.md), [`result-pattern`](../.agents/skills/result-pattern/SKILL.md) |
| 3 | Infrastructure | [`repository-pattern`](../.agents/skills/repository-pattern/SKILL.md), [`specification-pattern`](../.agents/skills/specification-pattern/SKILL.md), [`ef-core-configuration`](../.agents/skills/ef-core-configuration/SKILL.md), [`supabase-postgres-best-practices`](../.agents/skills/supabase-postgres-best-practices/SKILL.md) |
| 4 | API & Security | [`api-controller-generator`](../.agents/skills/api-controller-generator/SKILL.md), [`permission-authorization`](../.agents/skills/permission-authorization/SKILL.md) |
| 5 | Cross-Cutting | [`audit-trail`](../.agents/skills/audit-trail/SKILL.md), [`outbox-pattern`](../.agents/skills/outbox-pattern/SKILL.md), [`pipeline-behaviors`](../.agents/skills/pipeline-behaviors/SKILL.md), [`quartz-background-jobs`](../.agents/skills/quartz-background-jobs/SKILL.md), [`health-checks`](../.agents/skills/health-checks/SKILL.md) |
| 6 | Testing | [`unit-testing`](../.agents/skills/unit-testing/SKILL.md), [`integration-testing`](../.agents/skills/integration-testing/SKILL.md) |

## 4. Auto-invoke Skills

When performing these actions inside `Fintrack.Server/`, ALWAYS invoke the corresponding skill FIRST:

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

## 5. Coding Conventions
- **Application layout:** Under `Application/{Feature}/`, place each use case under **`Queries/{UseCase}/`** (reads) or **`Commands/{UseCase}/`** (writes), e.g. `Budgets/Queries/GetBudgets/`, `Budgets/Commands/UpsertBudgets/`. Namespace matches the path. See [`dotnet-clean-architecture`](../.agents/skills/dotnet-clean-architecture/SKILL.md).
- **Refactors:** When refactoring or extending behavior across layers, follow [`backend-refactor-quality-gate`](../.agents/skills/backend-refactor-quality-gate/SKILL.md) before considering the work complete.
- **CQRS & MediatR:** Keep controllers thin. All business logic must be delegated to MediatR Queries and Commands in the `Application` layer.
- **Repository Pattern:** Do not use `DbContext` directly in the `Application` layer. Use repositories to abstract data access.
- **Asynchronous Programming:** Always use `async`/`await` for I/O bound operations (database, HTTP calls). Suffix asynchronous methods with `Async`.
- **Dependency Injection:** Use constructor injection for all dependencies.
- **Validation:** Use FluentValidation in the `Application` layer to validate Commands and Queries.
- **Nullability:** Nullable reference types are enabled (`<Nullable>enable</Nullable>`). Ensure proper null handling and use `?` appropriately.
- **Testing Requirements:** Unit and Integration testing are STRICTLY MANDATORY for every new feature. A feature is not complete until Step 6 is fully executed. **Crucial Routing Rule:** NEVER place tests inside the `Fintrack.Server` project. Unit tests MUST be created inside `Fintrack.Tests/` and Integration tests MUST be created inside `Fintrack.IntegrationTests/`.
