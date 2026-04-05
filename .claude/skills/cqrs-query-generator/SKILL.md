---
name: cqrs-query-generator
description: "Generates CQRS Queries with MediatR handlers and response DTOs for read operations. Default data access is Entity Framework Core via abstractions implemented in Infrastructure; Dapper is optional when raw SQL is justified."
version: 1.2.0
language: C#
framework: .NET 8+
dependencies: MediatR, FluentValidation, Entity Framework Core (optional: Dapper)
---

# CQRS Query Generator

## Overview

This skill generates Queries following the CQRS pattern. Queries are read-side operations that return data without modifying state (except where a product decision explicitly performs a side effect—prefer separate commands for writes). Key principles:

- **Queries return DTOs, not domain entities** — Projection to response models at the application boundary
- **Prefer Entity Framework Core** — Implement reads in Infrastructure using EF (`DbContext`, `Include`/`Select`, `AsNoTracking` when appropriate) behind an interface the Application layer consumes
- **Keep `DbContext` out of query handlers** — Handlers depend on repository or read-service abstractions; Infrastructure registers EF implementations
- **Dapper is optional** — Use hand-written SQL/Dapper when profiling or complexity warrants it, not by default
- **One folder per query** — Under `Application/{Feature}/`, each query lives in its **own** folder (e.g. `GetBudgets/`). Do **not** use a shared `Queries/` folder. See [`dotnet-clean-architecture`](./dotnet-clean-architecture/SKILL.md) (**Application `{Feature}` folder layout**).

## Quick Reference

| Query Type | Use Case | Returns |
|------------|----------|---------|
| GetById | Single entity by ID | `Result<EntityResponse>` |
| GetAll | All entities (with optional filtering) | `Result<IReadOnlyList<EntityResponse>>` |
| GetPaged | Paginated list | `Result<PagedList<EntityResponse>>` |
| Search | Filtered/searched results | `Result<IReadOnlyList<EntityResponse>>` |
| Exists | Check if entity exists | `Result<bool>` |

## Read-side data access (default: EF Core)

1. Define an abstraction in **Application** (e.g. `Application/Abstractions/{Feature}/I{Entity}Repository.cs` or `I{Feature}ReadService`) that returns **DTOs** or `Result<T>` of DTOs.
2. Implement it in **Infrastructure** with **EF Core** — project with `.Select()`, use `.AsNoTracking()` for pure reads, avoid loading full aggregates when only a flat DTO is needed.
3. Query **handlers** inject only that abstraction — no `ApplicationDbContext` in Application.
4. **Dapper** (or ADO.NET): add only when EF is awkward or too slow for a specific hot path; keep SQL parameterized and inside Infrastructure.

**Interface placement:** If methods return application query DTOs, the interface belongs in **Application.Abstractions** so **Domain** does not reference Application types. Classic `I{Aggregate}Repository` with only entity + `Add`/`Update`/`Remove` may still live in Domain per project convention.

## Query Structure

**Mandatory layout:** `Application/{Feature}/{QueryFolder}/` only — no `Application/{Feature}/Queries/...`.

```
/Application/{Feature}/
├── Get{Entity}ById/
│   ├── Get{Entity}ByIdQuery.cs       # Query + Validator + Handler
│   └── {Entity}Response.cs            # Response DTO
├── GetAll{Entities}/
│   ├── GetAll{Entities}Query.cs
│   └── {Entity}ListResponse.cs
└── Get{Entities}ByOrganization/
    ├── Get{Entities}ByOrganizationQuery.cs
    └── {Entity}ByOrganizationResponse.cs
```

---

## Template: Get By ID Query (EF Core via abstraction)

```csharp
// src/{name}.application/Abstractions/{Feature}/I{Entity}Repository.cs
using {name}.application.{feature}.Get{Entity}ById;
using {name}.domain.abstractions;

namespace {name}.application.abstractions.{feature};

public interface I{Entity}Repository
{
    Task<Result<{Entity}Response?>> Get{Entity}ResponseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

// src/{name}.application/{Feature}/Get{Entity}ById/Get{Entity}ByIdQuery.cs
using FluentValidation;
using {name}.application.abstractions.{feature};
using {name}.application.abstractions.messaging;
using {name}.domain.abstractions;

namespace {name}.application.{feature}.Get{Entity}ById;

public sealed record Get{Entity}ByIdQuery(Guid Id) : IQuery<{Entity}Response>;

internal sealed class Get{Entity}ByIdQueryValidator : AbstractValidator<Get{Entity}ByIdQuery>
{
    public Get{Entity}ByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

internal sealed class Get{Entity}ByIdQueryHandler
    : IQueryHandler<Get{Entity}ByIdQuery, {Entity}Response>
{
    private readonly I{Entity}Repository _repository;

    public Get{Entity}ByIdQueryHandler(I{Entity}Repository repository)
    {
        _repository = repository;
    }

    public async Task<Result<{Entity}Response>> Handle(
        Get{Entity}ByIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.Get{Entity}ResponseByIdAsync(request.Id, cancellationToken);
        if (result.IsFailure)
            return Result.Failure<{Entity}Response>(result.Error);
        if (result.Value is null)
            return Result.Failure<{Entity}Response>({Entity}Errors.NotFound);
        return result.Value;
    }
}
```

```csharp
// src/{name}.infrastructure/Repositories/{Entity}Repository.cs (excerpt)
using Microsoft.EntityFrameworkCore;
using {name}.application.abstractions.{feature};
using {name}.application.{feature}.Get{Entity}ById;
using {name}.domain.abstractions;
using {name}.infrastructure.data;

namespace {name}.infrastructure.repositories;

internal sealed class {Entity}Repository : I{Entity}Repository
{
    private readonly ApplicationDbContext _dbContext;

    public {Entity}Repository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<{Entity}Response?>> Get{Entity}ResponseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var row = await _dbContext.{Entities}
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new {Entity}Response
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return row;
    }
}
```

### Response DTO

```csharp
// src/{name}.application/{Feature}/Get{Entity}ById/{Entity}Response.cs
namespace {name}.application.{feature}.Get{Entity}ById;

public sealed class {Entity}Response
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
```

List, paged, and report queries follow the same pattern: add methods to the same repository (or a dedicated read interface), implement with EF projections/`Include` as needed, keep handlers thin.

---

## Optional: Dapper for specific reads

When you choose Dapper (complex reporting, bulk exports, or measured performance need):

- Keep **SQL in Infrastructure** and use **parameterized** commands only.
- Handlers still depend on an abstraction (e.g. `I{Report}SqlQuery`) implemented with `IDbConnection`.
- Register a small `ISqlConnectionFactory` if the project does not already have one.

Example handler shape:

```csharp
internal sealed class {ReportName}QueryHandler
    : IQueryHandler<{ReportName}Query, IReadOnlyList<{ReportName}Response>>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public async Task<Result<IReadOnlyList<{ReportName}Response>>> Handle(
        {ReportName}Query request,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<{ReportName}Response>(
            /* parameterized SQL */,
            new { request.OrganizationId });
        return rows.ToList();
    }
}
```

---

## Optional: SQL connection factory (Dapper)

```csharp
// src/{name}.application/Abstractions/Data/ISqlConnectionFactory.cs
using System.Data;

namespace {name}.application.abstractions.data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}

// src/{name}.infrastructure/Data/SqlConnectionFactory.cs
using System.Data;
using Npgsql;
using {name}.application.abstractions.data;

namespace {name}.infrastructure.data;

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString) => _connectionString = connectionString;

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
```

---

## SQL best practices (raw SQL / Dapper only)

### Column naming (snake case to PascalCase)

```sql
SELECT 
    e.id AS Id,
    e.first_name AS FirstName,
    e.created_at AS CreatedAt
FROM entity e
```

### Avoiding N+1

Prefer one query with JOINs or CTEs over many round-trips.

### Using CTEs for complex queries

Use readable `WITH` clauses for ranking, windows, and multi-step filters.

---

## Critical Rules

1. **Queries should not modify state** — Use commands for writes; if a read path must persist, document it and consider refactoring to a command.
2. **Prefer EF Core for reads** in this codebase — Via Infrastructure implementations behind Application abstractions; use `AsNoTracking()` for read-only projections.
3. **Return DTOs, not entities** — Do not expose domain models from query handlers.
4. **No `DbContext` in Application** — Only abstractions + handlers + validators + DTOs.
5. **Dapper when justified** — Not the default; use for heavy SQL or proven bottlenecks.
6. **Parameterized SQL** — Whenever using Dapper/raw SQL, never concatenate user input into SQL strings.
7. **Validate query inputs** — FluentValidation on filters, pagination, date ranges.
8. **Handle not found** — Return `Result.Failure` with a typed domain/application error.

---

## Anti-patterns to avoid

```csharp
// ❌ WRONG: DbContext inside the Application query handler
internal sealed class GetEntityQueryHandler : IQueryHandler<...>
{
    private readonly ApplicationDbContext _dbContext; // leaks Infrastructure into Application
}

// ✅ CORRECT: Inject a repository/read abstraction implemented with EF in Infrastructure
internal sealed class GetEntityQueryHandler : IQueryHandler<...>
{
    private readonly IEntityRepository _repository;
}

// ❌ WRONG: Returning domain entities from queries
public sealed record GetEntityQuery(Guid Id) : IQuery<Entity>;

// ✅ CORRECT: Return DTOs / response records
public sealed record GetEntityQuery(Guid Id) : IQuery<EntityResponse>;

// ❌ WRONG: String concatenation in SQL (Dapper/raw SQL)
var sql = $"SELECT * FROM entity WHERE name = '{request.Name}'";

// ✅ CORRECT: Parameters
await connection.QueryAsync(sql, new { request.Name });
```

---

## Related Skills

- `cqrs-command-generator` - Generate write-side commands
- `domain-entity-generator` - Generate domain entities
- `ef-core-configuration` - EF Core mappings and DbContext
- `repository-pattern` - Repository interfaces and EF implementations
- `result-pattern` - Error handling pattern
