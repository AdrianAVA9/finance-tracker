namespace Fintrack.Server.Application.DTOs.Vision;

public class ExtractedLineItemData
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Category { get; set; }
}
