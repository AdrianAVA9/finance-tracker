using System;

namespace Fintrack.Server.Application.DTOs.Vision;

public class ExtractedExpenseData
{
    public string? Merchant { get; set; }
    public DateTime? Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string? OverallCategory { get; set; }
    public string? InvoiceNumber { get; set; }
}
