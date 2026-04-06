namespace Fintrack.Server.Api.Controllers.Expenses;

public sealed record RequestExpenseItemData(Guid CategoryId, decimal ItemAmount, string? Description);

public sealed record CreateExpenseRequest(
    string Merchant,
    decimal TotalAmount,
    DateTime Date,
    string? InvoiceNumber,
    string? InvoiceImageUrl,
    List<RequestExpenseItemData> Items);

public sealed record UpdateExpenseRequest(
    decimal TotalAmount,
    DateTime Date,
    string? Merchant,
    string? InvoiceNumber,
    string? InvoiceImageUrl,
    List<RequestExpenseItemData> Items);
