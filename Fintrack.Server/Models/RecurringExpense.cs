using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Models.Enums;

namespace Fintrack.Server.Models
{
    public class RecurringExpense : BaseAuditableEntity
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Merchant { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public RecurringFrequency Frequency { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime NextProcessingDate { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual ExpenseCategory? Category { get; set; }
    }
}
