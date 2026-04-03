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

## 3. Server-Specific Skills
When working in this directory, leverage the following skills (located in `../.agents/skills/`), organized by the typical backend development flow:

**1. Feature Implementation (Application Layer)**
- [`cqrs-command-generator`](../.agents/skills/cqrs-command-generator/SKILL.md): Generating MediatR Commands and handlers.
- [`cqrs-query-generator`](../.agents/skills/cqrs-query-generator/SKILL.md): Generating MediatR Queries and handlers.
- [`fluent-validation`](../.agents/skills/fluent-validation/SKILL.md): Creating and enforcing validation rules for commands/queries.
- [`result-pattern`](../.agents/skills/result-pattern/SKILL.md): Standardized error handling and result wrapping.

**2. Data Access & Persistence (Infrastructure Layer)**
- [`repository-pattern`](../.agents/skills/repository-pattern/SKILL.md): Data access abstraction with EF Core.
- [`ef-core-configuration`](../.agents/skills/ef-core-configuration/SKILL.md): Configuring EF Core entity mappings (Fluent API).
- [`supabase-postgres-best-practices`](../.agents/skills/supabase-postgres-best-practices/SKILL.md): Database optimization and query best practices.

**3. API & Security (Presentation Layer)**
- [`api-controller-generator`](../.agents/skills/api-controller-generator/SKILL.md): Generating RESTful controllers with MediatR.
- [`permission-authorization`](../.agents/skills/permission-authorization/SKILL.md): Granular access control using custom attributes and policies.

**4. Cross-Cutting Concerns & Monitoring**
- [`audit-trail`](../.agents/skills/audit-trail/SKILL.md): Implementing entity audit logging.
- [`quartz-background-jobs`](../.agents/skills/quartz-background-jobs/SKILL.md): Creating scheduled background jobs.
- [`health-checks`](../.agents/skills/health-checks/SKILL.md): Implementing application and database health checks.

**5. Verification & Testing**
- [`unit-testing`](../.agents/skills/unit-testing/SKILL.md): AAA pattern using xUnit and NSubstitute.
- [`integration-testing`](../.agents/skills/integration-testing/SKILL.md): Testing with real dependencies using Testcontainers.

## 4. Coding Conventions
- **CQRS & MediatR:** Keep controllers thin. All business logic must be delegated to MediatR Queries and Commands in the `Application` layer.
- **Repository Pattern:** Do not use `DbContext` directly in the `Application` layer. Use repositories to abstract data access.
- **Asynchronous Programming:** Always use `async`/`await` for I/O bound operations (database, HTTP calls). Suffix asynchronous methods with `Async`.
- **Dependency Injection:** Use constructor injection for all dependencies.
- **Validation:** Use FluentValidation in the `Application` layer to validate Commands and Queries.
- **Nullability:** Nullable reference types are enabled (`<Nullable>enable</Nullable>`). Ensure proper null handling and use `?` appropriately.
