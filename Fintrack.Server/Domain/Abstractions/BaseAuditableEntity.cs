namespace Fintrack.Server.Domain.Abstractions;

public abstract class BaseAuditableEntity : Entity, IAuditableEntity
{
    protected BaseAuditableEntity(int id) : base(id) { }
    protected BaseAuditableEntity() : base() { }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
