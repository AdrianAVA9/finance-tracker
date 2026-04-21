using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Incomes.Events;

public sealed record IncomeCreatedDomainEvent(Guid IncomeId) : IDomainEvent;

public sealed record IncomeUpdatedDomainEvent(Guid IncomeId) : IDomainEvent;

public sealed record IncomeDeletedDomainEvent(Guid IncomeId) : IDomainEvent;
