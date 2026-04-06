using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.IncomeCategories.Events;

public sealed record IncomeCategoryCreatedDomainEvent(Guid IncomeCategoryId) : IDomainEvent;

public sealed record IncomeCategoryUpdatedDomainEvent(Guid IncomeCategoryId) : IDomainEvent;

public sealed record IncomeCategoryDeletedDomainEvent(Guid IncomeCategoryId) : IDomainEvent;
