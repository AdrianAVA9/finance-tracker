using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Invoices;

public class Invoice : Entity
{
    public string UserId { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? MerchantName { get; set; }
    public DateTime? Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ErrorMessage { get; set; }

    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
