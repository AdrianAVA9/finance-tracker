using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Models
{
    public class ExpenseCategory : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Color { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Added foreign key for ExpenseCategoryGroup
        public int? GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public virtual ExpenseCategoryGroup? Group { get; set; }

        public bool IsEditable { get; set; }

        // Audit Columns
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
