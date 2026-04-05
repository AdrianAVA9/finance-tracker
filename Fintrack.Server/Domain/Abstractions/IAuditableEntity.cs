namespace Fintrack.Server.Domain.Abstractions;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
    string? LastModifiedBy { get; set; }
}
