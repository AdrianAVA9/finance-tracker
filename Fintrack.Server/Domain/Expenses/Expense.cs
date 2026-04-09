using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Expenses.Events;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Expenses;

public sealed class Expense : BaseAuditableEntityGuid
{
    private readonly List<ExpenseItem> _items = new();

    public string UserId { get; private set; } = string.Empty;
    public Guid? ExpenseCategoryId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime Date { get; private set; }
    public string? Merchant { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public string? InvoiceImageUrl { get; private set; }
    public AiExtractionStatus AiExtractionStatus { get; private set; } = AiExtractionStatus.NotApplicable;
    public ExpenseStatus Status { get; private set; } = ExpenseStatus.Draft;
    public Guid? InvoiceId { get; private set; }

    public ApplicationUser? User { get; private set; }
    public ExpenseCategory? ExpenseCategory { get; private set; }
    public Invoice? Invoice { get; private set; }
    public IReadOnlyCollection<ExpenseItem> Items => _items.AsReadOnly();

    private Expense() : base()
    {
    }

    private Expense(
        string userId,
        decimal totalAmount,
        DateTime date,
        string? merchant,
        string? invoiceNumber,
        string? invoiceImageUrl,
        ExpenseStatus status,
        AiExtractionStatus aiExtractionStatus)
        : base()
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TotalAmount = totalAmount;
        Date = date;
        Merchant = merchant;
        InvoiceNumber = invoiceNumber;
        InvoiceImageUrl = invoiceImageUrl;
        Status = status;
        AiExtractionStatus = aiExtractionStatus;
    }

    public static Result<Expense> Create(
        string userId,
        decimal totalAmount,
        DateTime date,
        string? merchant,
        string? invoiceNumber = null,
        string? invoiceImageUrl = null,
        ExpenseStatus status = ExpenseStatus.Completed,
        AiExtractionStatus aiExtractionStatus = AiExtractionStatus.NotApplicable)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure<Expense>(ExpenseErrors.UserIdRequired);
        }

        if (totalAmount <= 0)
        {
            return Result.Failure<Expense>(ExpenseErrors.InvalidTotalAmount);
        }

        var expense = new Expense(
            userId,
            totalAmount,
            date,
            merchant,
            invoiceNumber,
            invoiceImageUrl,
            status,
            aiExtractionStatus);

        expense.RaiseDomainEvent(new ExpenseCreatedDomainEvent(expense.Id));

        return expense;
    }

    public Result Update(
        decimal totalAmount,
        DateTime date,
        string? merchant,
        string? invoiceNumber,
        string? invoiceImageUrl)
    {
        if (totalAmount <= 0)
        {
            return Result.Failure(ExpenseErrors.InvalidTotalAmount);
        }

        TotalAmount = totalAmount;
        Date = date;
        Merchant = merchant;
        InvoiceNumber = invoiceNumber;
        InvoiceImageUrl = invoiceImageUrl;

        RaiseDomainEvent(new ExpenseUpdatedDomainEvent(Id));

        return Result.Success();
    }

    public void AddItem(ExpenseItem item)
    {
        _items.Add(item);
    }

    public void ClearItems()
    {
        _items.Clear();
    }

    public void ReplaceItems(IEnumerable<ExpenseItem> newItems)
    {
        _items.Clear();
        _items.AddRange(newItems);
    }

    public void LinkInvoice(Invoice invoice)
    {
        Invoice = invoice;
    }

    public void ClearInvoiceLink()
    {
        Invoice = null;
    }

    public Result SetStatus(ExpenseStatus newStatus)
    {
        Status = newStatus;
        RaiseDomainEvent(new ExpenseUpdatedDomainEvent(Id));
        return Result.Success();
    }

    /// <summary>
    /// Registers that this expense will be removed. Raises <see cref="ExpenseDeletedDomainEvent"/>.
    /// Call immediately before <see cref="IExpenseRepository.Remove"/> so the event is dispatched with save.
    /// </summary>
    public void RegisterDeletion()
    {
        RaiseDomainEvent(new ExpenseDeletedDomainEvent(Id));
    }
}
