using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Budgets.Events;

public sealed record BudgetCreatedDomainEvent(int BudgetId) : IDomainEvent;

public sealed record BudgetUpdatedDomainEvent(int BudgetId) : IDomainEvent;

public sealed record BudgetDeletedDomainEvent(int BudgetId) : IDomainEvent;
