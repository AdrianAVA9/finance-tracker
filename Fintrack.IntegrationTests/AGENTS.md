# CeroBase Integration Tests - AI Agent Guide

This document provides specialized context and rules for AI agents working within the `Fintrack.IntegrationTests` directory. It overrides general knowledge and ensures code aligns with the established integration testing architecture and standards.

## 1. Context & Tech Stack
The `Fintrack.IntegrationTests` project is responsible for testing the `Fintrack.Server` application flow end-to-end (from the API boundary to the database).
- **Framework:** xUnit
- **App Hosting:** `WebApplicationFactory`
- **Database:** Testcontainers (PostgreSQL)

## 2. Architecture & Scope
- **Target:** Integration tests should exercise the entire application stack, including the database and HTTP endpoints.
- **Real Dependencies:** Use a real database (via Testcontainers) instead of mocks.
- **Authentication:** Use the provided test authentication handlers to bypass real token validation during testing.

## 3. Testing-Specific Skills
When working in this directory, leverage the following skill (located in `../.agents/skills/`):

- [`integration-testing`](../.agents/skills/integration-testing/SKILL.md): Testing with real dependencies using `WebApplicationFactory` and Testcontainers.

## 4. Coding Conventions
- **Base Class:** Tests should typically inherit from a base integration test class (e.g., `BaseIntegrationTest`) that manages the WebApplicationFactory and Testcontainers lifecycle.
- **Database State:** Ensure tests do not interfere with each other by resetting the database state or using isolated schemas/data per test class.
- **HTTP Client:** Use the `HttpClient` provided by the `WebApplicationFactory` to make requests to the API endpoints.
- **Naming:** Name test files after the feature or controller being tested (e.g., `BudgetsControllerTests.cs` or `CreateExpenseTests.cs`).
