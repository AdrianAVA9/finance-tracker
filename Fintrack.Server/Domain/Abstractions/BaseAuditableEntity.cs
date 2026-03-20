using System;
using System.ComponentModel.DataAnnotations;

namespace Fintrack.Server.Domain.Abstractions
{
    public abstract class BaseAuditableEntity : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
