using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Invoices.Events;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Invoices;

public sealed class Invoice : BaseAuditableEntityGuid
{
    private readonly List<InvoiceItem> _items = new();

    public string UserId { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public string? MerchantName { get; private set; }
    public DateTime? Date { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; } = "Pending";
    public string? ErrorMessage { get; private set; }

    public ApplicationUser? User { get; private set; }
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    private Invoice() : base()
    {
    }

    private Invoice(
        string userId,
        string? imageUrl,
        string? merchantName,
        DateTime? date,
        decimal totalAmount,
        string status,
        string? errorMessage)
        : base()
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ImageUrl = imageUrl;
        MerchantName = merchantName;
        Date = date;
        TotalAmount = totalAmount;
        Status = status;
        ErrorMessage = errorMessage;
    }

    public static Result<Invoice> Create(
        string userId,
        decimal totalAmount,
        string? imageUrl = null,
        string? merchantName = null,
        DateTime? date = null,
        string status = "Pending",
        string? errorMessage = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure<Invoice>(InvoiceErrors.UserIdRequired);
        }

        if (totalAmount < 0)
        {
            return Result.Failure<Invoice>(InvoiceErrors.NegativeTotalAmount);
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            return Result.Failure<Invoice>(InvoiceErrors.StatusRequired);
        }

        var invoice = new Invoice(
            userId.Trim(),
            imageUrl,
            merchantName,
            date,
            totalAmount,
            status.Trim(),
            errorMessage);

        invoice.RaiseDomainEvent(new InvoiceCreatedDomainEvent(invoice.Id));

        return invoice;
    }

    public void AddItem(InvoiceItem item)
    {
        _items.Add(item);
    }

    public Result UpdateDetails(
        string? imageUrl,
        string? merchantName,
        DateTime? date,
        decimal totalAmount,
        string status,
        string? errorMessage = null)
    {
        if (totalAmount < 0)
        {
            return Result.Failure(InvoiceErrors.NegativeTotalAmount);
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            return Result.Failure(InvoiceErrors.StatusRequired);
        }

        ImageUrl = imageUrl;
        MerchantName = merchantName;
        Date = date;
        TotalAmount = totalAmount;
        Status = status.Trim();
        ErrorMessage = errorMessage;
        return Result.Success();
    }

    public void ReplaceLineItems(IEnumerable<InvoiceItem> newItems)
    {
        _items.Clear();
        _items.AddRange(newItems);
    }
}
