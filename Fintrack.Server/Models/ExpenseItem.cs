using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Models
{
    public class ExpenseItem : BaseAuditableEntity
    {
        public int ExpenseId { get; set; }

        public int CategoryId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ItemAmount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [ForeignKey(nameof(ExpenseId))]
        public virtual Expense? Expense { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual ExpenseCategory? Category { get; set; }
    }
}
