using System;

namespace Fintrack.Server.Application.DTOs.Vision;

public class ExtractedInvoiceData
{
    public string? MerchantName { get; set; }
    public DateTime? Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string? InvoiceNumber { get; set; }
}
