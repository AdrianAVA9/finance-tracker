using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Models.Enums;

namespace Fintrack.Server.Models
{
    public class Expense : BaseAuditableEntity
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime Date { get; set; }

        [StringLength(200)]
        public string? Merchant { get; set; }

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        [StringLength(500)]
        public string? InvoiceImageUrl { get; set; }

        public AiExtractionStatus AiExtractionStatus { get; set; } = AiExtractionStatus.NotApplicable;

        public ExpenseStatus Status { get; set; } = ExpenseStatus.Draft;

        public int? InvoiceId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public virtual Invoice? Invoice { get; set; }

        public virtual ICollection<ExpenseItem> Items { get; set; } = new List<ExpenseItem>();
    }
}
