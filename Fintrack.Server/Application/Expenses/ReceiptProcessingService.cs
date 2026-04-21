using Fintrack.Server.Application.Abstractions.Vision;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Invoices;

namespace Fintrack.Server.Application.Expenses;

public class ReceiptProcessingService
{
    private readonly IVisionExtractionProvider _visionProvider;
    private readonly IExpenseCategoryRepository _categoryRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceiptProcessingService(
        IVisionExtractionProvider visionProvider,
        IExpenseCategoryRepository categoryRepository,
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork)
    {
        _visionProvider = visionProvider;
        _categoryRepository = categoryRepository;
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Expense> ProcessReceiptAsync(
        Stream imageStream,
        string mimeType,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var userCategories = await _categoryRepository.GetAllByUserIdAsync(userId, cancellationToken);
        var categoryNames = userCategories.Select(c => c.Name).ToList();

        var extractedData = await _visionProvider.ExtractReceiptDataAsync(
            imageStream,
            mimeType,
            categoryNames,
            cancellationToken);

        var invoiceTotal = extractedData.Invoice.TotalAmount > 0
            ? extractedData.Invoice.TotalAmount
            : extractedData.Expense.TotalAmount;

        var invoiceResult = Invoice.Create(
            userId,
            invoiceTotal,
            imageUrl: null,
            merchantName: extractedData.Invoice.MerchantName ?? extractedData.Expense.Merchant,
            date: extractedData.Invoice.Date ?? extractedData.Expense.Date);

        if (invoiceResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to create invoice from receipt: {invoiceResult.Error.Description}");
        }

        var invoice = invoiceResult.Value;

        var totalAmount = extractedData.Expense.TotalAmount > 0
            ? extractedData.Expense.TotalAmount
            : invoice.TotalAmount;

        var expenseResult = Expense.Create(
            userId,
            totalAmount,
            extractedData.Expense.Date ?? invoice.Date ?? DateTime.UtcNow,
            extractedData.Expense.Merchant ?? invoice.MerchantName,
            extractedData.Expense.InvoiceNumber ?? extractedData.Invoice.InvoiceNumber,
            status: ExpenseStatus.NeedsReview,
            aiExtractionStatus: AiExtractionStatus.Completed);

        if (expenseResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to create expense from receipt: {expenseResult.Error.Description}");
        }

        var expense = expenseResult.Value;
        expense.LinkInvoice(invoice);

        var categoryDict = userCategories
            .ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

        Guid overallCategoryId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(extractedData.Expense.OverallCategory) &&
            categoryDict.TryGetValue(extractedData.Expense.OverallCategory, out var oId))
        {
            overallCategoryId = oId;
        }

        foreach (var extractedItem in extractedData.LineItems)
        {
            Guid categoryId = overallCategoryId;
            if (!string.IsNullOrWhiteSpace(extractedItem.Category) &&
                categoryDict.TryGetValue(extractedItem.Category, out var id))
            {
                categoryId = id;
            }

            if (categoryId == Guid.Empty)
            {
                categoryId = userCategories.FirstOrDefault()?.Id ?? Guid.Empty;
            }

            var itemAmount = extractedItem.TotalPrice > 0
                ? extractedItem.TotalPrice
                : extractedItem.Quantity * extractedItem.UnitPrice;

            expense.AddItem(ExpenseItem.Create(categoryId, itemAmount, extractedItem.Description));

            var quantity = extractedItem.Quantity > 0 ? extractedItem.Quantity : 1;
            var lineDescription = string.IsNullOrWhiteSpace(extractedItem.Description)
                ? "Item"
                : extractedItem.Description;
            var invoiceItemResult = InvoiceItem.Create(
                lineDescription,
                quantity,
                extractedItem.UnitPrice,
                extractedItem.TotalPrice,
                categoryId == Guid.Empty ? null : categoryId);

            if (invoiceItemResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Failed to create invoice line item: {invoiceItemResult.Error.Description}");
            }

            invoice.AddItem(invoiceItemResult.Value);
        }

        _expenseRepository.Add(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return expense;
    }
}
