using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Expenses.Events;

public sealed record ExpenseCreatedDomainEvent(Guid ExpenseId) : IDomainEvent;

public sealed record ExpenseUpdatedDomainEvent(Guid ExpenseId) : IDomainEvent;

public sealed record ExpenseDeletedDomainEvent(Guid ExpenseId) : IDomainEvent;
