# CeroBase - AI Agent Guide

This repository contains the **CeroBase** project, a web-based personal budget application. This `AGENTS.md` file serves as a centralized map for AI assistants working within this monorepo to ensure consistency across the different components.

## 1. Project Overview
CeroBase tracks income and expenses, manages monthly budgets, and sets savings goals. It offers dashboards and reports for financial clarity, helping users control spending and make better financial decisions securely and efficiently.

## 2. Component Mapping
The project is divided into the following primary directories, each with its specific technology stack:

- **`Fintrack.Client/` (Frontend UI)**
  - **Stack:** Vue 3, TypeScript, Pinia, Vue Router, Tailwind CSS, Vite.
  - **Rules:** Adhere to the Vue Client Architecture.

- **`Fintrack.Server/` (Backend API)**
  - **Stack:** .NET 8, C#, ASP.NET Core, Entity Framework Core, PostgreSQL, MediatR.
  - **Rules:** Follow Clean Architecture principles and the Repository pattern.

- **`Fintrack.Tests/` & `Fintrack.IntegrationTests/` (Testing)**
  - **Stack:** xUnit, NSubstitute, WebApplicationFactory, Testcontainers.

## 3. Agent Skills
This repository contains specialized instructions (skills) in `.agents/skills/` designed to assist with specific tasks. Agents should invoke these skills based on the task context:

### Frontend (Vue.js)
- [`vue-best-practices`](.agents/skills/vue-best-practices/SKILL.md): Core guidelines for Vue 3 (Composition API).
- [`vue-client-architecture`](.agents/skills/vue-client-architecture/SKILL.md): Enforces 4-domain client architecture (public, auth, app, admin).
- [`vue-router-best-practices`](.agents/skills/vue-router-best-practices/SKILL.md) / [`vue-pinia-best-practices`](.agents/skills/vue-pinia-best-practices/SKILL.md): State and routing guidelines.
- [`vue-testing-best-practices`](.agents/skills/vue-testing-best-practices/SKILL.md): Testing with Vitest and Vue Test Utils.
- [`vue-debug-guides`](.agents/skills/vue-debug-guides/SKILL.md): Handling runtime errors, hydration issues, etc.
- [`create-adaptable-composable`](.agents/skills/create-adaptable-composable/SKILL.md): Standards for writing robust composables.
- [`pwa-development`](.agents/skills/pwa-development/SKILL.md): Progressive Web Apps guidelines.

### Backend (.NET)
*Organized by typical development flow:*

**1. Enterprise Business Rules (Domain Layer)**
- [`domain-entity-generator`](.agents/skills/domain-entity-generator/SKILL.md): Generating Domain Entities following strict DDD principles.
- [`domain-events-generator`](.agents/skills/domain-events-generator/SKILL.md): Generating Domain Events and their MediatR handlers.

**2. Feature Implementation (Application Layer)**
- [`cqrs-command-generator`](.agents/skills/cqrs-command-generator/SKILL.md): Generating MediatR Commands and handlers.
- [`cqrs-query-generator`](.agents/skills/cqrs-query-generator/SKILL.md): Generating MediatR Queries and handlers.
- [`fluent-validation`](.agents/skills/fluent-validation/SKILL.md): Creating and enforcing validation rules.
- [`result-pattern`](.agents/skills/result-pattern/SKILL.md): Standardized error handling and result wrapping.

**3. Data Access & Persistence (Infrastructure Layer)**
- [`repository-pattern`](.agents/skills/repository-pattern/SKILL.md): Data access abstraction with EF Core.
- [`specification-pattern`](.agents/skills/specification-pattern/SKILL.md): Encapsulating reusable query logic.
- [`ef-core-configuration`](.agents/skills/ef-core-configuration/SKILL.md): Configuring EF Core entity mappings (Fluent API).
- [`supabase-postgres-best-practices`](.agents/skills/supabase-postgres-best-practices/SKILL.md): Database optimization and queries.

**4. API & Security (Presentation Layer)**
- [`api-controller-generator`](.agents/skills/api-controller-generator/SKILL.md): RESTful controller generation with MediatR.
- [`permission-authorization`](.agents/skills/permission-authorization/SKILL.md): Granular access control using custom attributes and policies.

**5. Cross-Cutting Concerns**
- [`audit-trail`](.agents/skills/audit-trail/SKILL.md): Implementing entity audit logging.
- [`outbox-pattern`](.agents/skills/outbox-pattern/SKILL.md): Ensuring reliable domain event processing.
- [`quartz-background-jobs`](.agents/skills/quartz-background-jobs/SKILL.md): Creating scheduled background jobs.
- [`health-checks`](.agents/skills/health-checks/SKILL.md): Implementing application and database health checks.

**6. Verification & Testing**
- [`unit-testing`](.agents/skills/unit-testing/SKILL.md): AAA pattern using xUnit and NSubstitute.
- [`integration-testing`](.agents/skills/integration-testing/SKILL.md): Real dependencies using Testcontainers.

### General & Workflow
- [`brainstorming`](.agents/skills/brainstorming/SKILL.md): Used prior to creative work to define intent and requirements.
- [`conventional-commit`](.agents/skills/conventional-commit/SKILL.md): Generating standard, descriptive commit messages.
- [`documentation-writer`](.agents/skills/documentation-writer/SKILL.md): Writing technical documentation (Diátaxis framework).
- [`excalidraw-diagram-generator`](.agents/skills/excalidraw-diagram-generator/SKILL.md): Visualizing processes and architecture.
- [`skill-creator`](.agents/skills/skill-creator/SKILL.md): Creating or updating new skills.

## 4. Development Norms
- **Commit Messages:** Must strictly follow the Conventional Commits specification.
- **Frontend Practices:** ALWAYS use Composition API (`<script setup>`) and TypeScript. Avoid Options API unless explicitly requested.
- **Backend Practices:** Avoid putting business logic directly in controllers; delegate to MediatR handlers and maintain type safety.
- **Testing Requirements:** Unit and Integration testing are STRICTLY MANDATORY for every new feature. A feature is not complete until Step 6 is fully executed.
