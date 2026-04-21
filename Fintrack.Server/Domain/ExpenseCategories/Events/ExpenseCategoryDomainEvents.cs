using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.ExpenseCategories.Events;

public sealed record ExpenseCategoryCreatedDomainEvent(Guid ExpenseCategoryId) : IDomainEvent;

public sealed record ExpenseCategoryUpdatedDomainEvent(Guid ExpenseCategoryId) : IDomainEvent;

public sealed record ExpenseCategoryDeletedDomainEvent(Guid ExpenseCategoryId) : IDomainEvent;
