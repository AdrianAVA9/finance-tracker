using Fintrack.Server.Domain.Enums;

namespace Fintrack.Server.Api.Controllers.Expenses;

public sealed record RequestExpenseItemData(Guid CategoryId, decimal ItemAmount, string? Description);

public sealed record ExpenseInvoiceLineRequest(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? AssignedCategoryId);

public sealed record ExpenseInvoiceRequest(
    string? ImageUrl,
    string? MerchantName,
    DateTime? Date,
    decimal TotalAmount,
    string? Status,
    List<ExpenseInvoiceLineRequest> Lines);

public sealed record CreateExpenseRequest(
    string Merchant,
    decimal TotalAmount,
    DateTime Date,
    string? InvoiceNumber,
    string? InvoiceImageUrl,
    List<RequestExpenseItemData> Items,
    ExpenseInvoiceRequest? Invoice = null);

public sealed record UpdateExpenseRequest(
    decimal TotalAmount,
    DateTime Date,
    string? Merchant,
    string? InvoiceNumber,
    string? InvoiceImageUrl,
    List<RequestExpenseItemData> Items,
    bool RemoveInvoice = false,
    ExpenseInvoiceRequest? Invoice = null,
    ExpenseStatus? Status = null);
