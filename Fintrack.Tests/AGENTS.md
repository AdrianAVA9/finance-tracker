# CeroBase Unit Tests - AI Agent Guide

This document provides specialized context and rules for AI agents working within the `Fintrack.Tests` directory. It overrides general knowledge and ensures code aligns with the established unit testing architecture and standards.

## 1. Context & Tech Stack
The `Fintrack.Tests` project is responsible for isolated unit testing of the `Fintrack.Server` logic (primarily the Application and Domain layers).
- **Framework:** xUnit
- **Mocking Library:** NSubstitute
- **Test Pattern:** AAA (Arrange, Act, Assert)

## 2. Architecture & Scope
- **Target:** Unit tests should test a single class or method in isolation.
- **Mocks:** External dependencies (Repositories, Email senders, etc.) MUST be mocked using NSubstitute. Do not connect to a real database.
- **Structure:** Mirror the folder structure of the code being tested (e.g., tests for `Application/Budgets` should be in `Fintrack.Tests/Application/Budgets/`).

## 3. Testing-Specific Skills
When working in this directory, leverage the following skill (located in `../.agents/skills/`):

- [`unit-testing`](../.agents/skills/unit-testing/SKILL.md): Enforces the AAA pattern using xUnit and NSubstitute.

## 4. Coding Conventions
- **Naming:** Name test files after the class under test, suffixed with `Tests` (e.g., `CreateBudgetCommandHandlerTests.cs`).
- **Method Naming:** Use clear names that describe the scenario being tested. Example: `MethodName_StateUnderTest_ExpectedBehavior`.
- **AAA Pattern:** Explicitly separate the `Arrange`, `Act`, and `Assert` phases of your tests.
- **SUT Setup:** Create the System Under Test (SUT) in the test method or constructor using mocks for all constructor parameters.
