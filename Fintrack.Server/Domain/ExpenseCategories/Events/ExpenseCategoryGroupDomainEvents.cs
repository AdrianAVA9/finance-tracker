using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.ExpenseCategories.Events;

public sealed record ExpenseCategoryGroupCreatedDomainEvent(Guid ExpenseCategoryGroupId) : IDomainEvent;

public sealed record ExpenseCategoryGroupUpdatedDomainEvent(Guid ExpenseCategoryGroupId) : IDomainEvent;

public sealed record ExpenseCategoryGroupDeletedDomainEvent(Guid ExpenseCategoryGroupId) : IDomainEvent;
