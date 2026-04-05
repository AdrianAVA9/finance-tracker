using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Budgets.Events;

public sealed record BudgetCreatedDomainEvent(Guid BudgetId) : IDomainEvent;

public sealed record BudgetUpdatedDomainEvent(Guid BudgetId) : IDomainEvent;

public sealed record BudgetDeletedDomainEvent(Guid BudgetId) : IDomainEvent;
