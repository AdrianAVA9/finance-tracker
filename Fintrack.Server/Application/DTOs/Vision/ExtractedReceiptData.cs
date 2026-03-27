namespace Fintrack.Server.Application.DTOs.Vision;

public class ExtractedReceiptData
{
    public ExtractedInvoiceData Invoice { get; set; } = new();
    public ExtractedExpenseData Expense { get; set; } = new();
    public List<ExtractedLineItemData> LineItems { get; set; } = new();
}
