using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fintrack.Server.Models
{
    public class InvoiceItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Nullable category ID if auto-categorized
        public int? AssignedCategoryId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public virtual Invoice? Invoice { get; set; }

        [ForeignKey(nameof(AssignedCategoryId))]
        public virtual ExpenseCategory? AssignedCategory { get; set; }
    }
}
