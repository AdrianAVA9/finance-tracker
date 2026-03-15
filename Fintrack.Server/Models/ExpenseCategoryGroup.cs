using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Models
{
    public class ExpenseCategoryGroup : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsEditable { get; set; }

        // Audit Columns
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<ExpenseCategory> Categories { get; set; } = new List<ExpenseCategory>();
    }
}
