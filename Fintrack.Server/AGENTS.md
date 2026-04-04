# CeroBase Server - AI Agent Guide

This document provides specialized context and rules for AI agents working within the `Fintrack.Server` directory. It overrides general knowledge and ensures code aligns with the established backend architecture and standards.

## 1. Context & Tech Stack
The `Fintrack.Server` is a RESTful API built with:
- **Framework:** .NET 8, ASP.NET Core
- **Language:** C#
- **Database ORM:** Entity Framework Core (EF Core)
- **Database Provider:** PostgreSQL (Npgsql)
- **Architecture Pattern:** Clean Architecture, CQRS (via MediatR)
- **Testing:** xUnit, NSubstitute, WebApplicationFactory, Testcontainers

## 2. Architecture: Clean Architecture
The backend adheres strictly to Clean Architecture principles. Agents MUST respect these boundaries:
- **`Domain/`**: Enterprise logic and entities. No dependencies on outer layers.
- **`Application/`**: Business logic, use cases (CQRS handlers), and DTOs. Depends only on `Domain`.
- **`Infrastructure/`**: Data access, external services, and identity implementations.
- **`Api/`** (or `Controllers/`): The presentation layer. HTTP endpoints, middleware, and dependency injection setup.

## 3. Mandatory Development Workflow & Skills
Every new backend feature MUST be implemented following these 6 sequential steps. You must not skip steps. **Testing (Step 6) is strictly mandatory for every feature.** Leverage the following skills (located in `../.agents/skills/`) for each step:

**1. Enterprise Business Rules (Domain Layer)**
- [`domain-entity-generator`](../.agents/skills/domain-entity-generator/SKILL.md): Generating Domain Entities following strict DDD principles.
- [`domain-events-generator`](../.agents/skills/domain-events-generator/SKILL.md): Generating Domain Events and their MediatR handlers.

**2. Feature Implementation (Application Layer)**
- [`cqrs-command-generator`](../.agents/skills/cqrs-command-generator/SKILL.md): Generating MediatR Commands and handlers.
- [`cqrs-query-generator`](../.agents/skills/cqrs-query-generator/SKILL.md): Generating MediatR Queries and handlers.
- [`fluent-validation`](../.agents/skills/fluent-validation/SKILL.md): Creating and enforcing validation rules for commands/queries.
- [`result-pattern`](../.agents/skills/result-pattern/SKILL.md): Standardized error handling and result wrapping.

**3. Data Access & Persistence (Infrastructure Layer)**
- [`repository-pattern`](../.agents/skills/repository-pattern/SKILL.md): Data access abstraction with EF Core.
- [`specification-pattern`](../.agents/skills/specification-pattern/SKILL.md): Encapsulating reusable query logic.
- [`ef-core-configuration`](../.agents/skills/ef-core-configuration/SKILL.md): Configuring EF Core entity mappings (Fluent API).
- [`supabase-postgres-best-practices`](../.agents/skills/supabase-postgres-best-practices/SKILL.md): Database optimization and query best practices.

**4. API & Security (Presentation Layer)**
- [`api-controller-generator`](../.agents/skills/api-controller-generator/SKILL.md): Generating RESTful controllers with MediatR.
- [`permission-authorization`](../.agents/skills/permission-authorization/SKILL.md): Granular access control using custom attributes and policies.

**5. Cross-Cutting Concerns & Monitoring**
- [`audit-trail`](../.agents/skills/audit-trail/SKILL.md): Implementing entity audit logging.
- [`outbox-pattern`](../.agents/skills/outbox-pattern/SKILL.md): Ensuring reliable domain event processing.
- [`pipeline-behaviors`](../.agents/skills/pipeline-behaviors/SKILL.md): MediatR behaviors for logging, validation, and performance.
- [`quartz-background-jobs`](../.agents/skills/quartz-background-jobs/SKILL.md): Creating scheduled background jobs.
- [`health-checks`](../.agents/skills/health-checks/SKILL.md): Implementing application and database health checks.

**6. Verification & Testing**
- [`unit-testing`](../.agents/skills/unit-testing/SKILL.md): AAA pattern using xUnit and NSubstitute.
- [`integration-testing`](../.agents/skills/integration-testing/SKILL.md): Testing with real dependencies using Testcontainers.

## 4. Coding Conventions
- **CQRS & MediatR:** Keep controllers thin. All business logic must be delegated to MediatR Queries and Commands in the `Application` layer.
- **Repository Pattern:** Do not use `DbContext` directly in the `Application` layer. Use repositories to abstract data access.
- **Asynchronous Programming:** Always use `async`/`await` for I/O bound operations (database, HTTP calls). Suffix asynchronous methods with `Async`.
- **Dependency Injection:** Use constructor injection for all dependencies.
- **Validation:** Use FluentValidation in the `Application` layer to validate Commands and Queries.
- **Nullability:** Nullable reference types are enabled (`<Nullable>enable</Nullable>`). Ensure proper null handling and use `?` appropriately.
- **Testing Requirements:** Unit and Integration testing are STRICTLY MANDATORY for every new feature. A feature is not complete until Step 6 is fully executed. **Crucial Routing Rule:** NEVER place tests inside the `Fintrack.Server` project. Unit tests MUST be created inside `Fintrack.Tests/` and Integration tests MUST be created inside `Fintrack.IntegrationTests/`.
