using System.Text.Json.Serialization;

namespace Fintrack.Server.Domain.Abstractions;

/// <summary>
/// Temporary base for aggregates that use <see cref="Guid"/> primary keys with audit fields and domain events.
/// When every aggregate is migrated off <see cref="int"/> ids, merge this into <see cref="BaseAuditableEntity"/>
/// (or a single generic base), remove <see cref="BaseAuditableEntity"/>, and rename as needed.
/// </summary>
public abstract class BaseAuditableEntityGuid : IAuditableEntity, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    [JsonInclude]
    public Guid Id { get; protected set; }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? LastModifiedBy { get; set; }

    protected BaseAuditableEntityGuid()
    {
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
