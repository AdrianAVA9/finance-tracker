using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Budgets;

/// <summary>
/// Budget-domain aggregate root base: Guid identifier with audit fields and domain events.
/// </summary>
public abstract class BudgetAuditableEntity : IAuditableEntity, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; protected set; }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? LastModifiedBy { get; set; }

    protected BudgetAuditableEntity()
    {
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
