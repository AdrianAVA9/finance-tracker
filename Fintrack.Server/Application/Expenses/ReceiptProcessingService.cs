using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fintrack.Server.Application.Abstractions.Vision;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;

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
        // 1. Get user categories
        var userCategories = await _categoryRepository.GetAllByUserIdAsync(userId, cancellationToken);
        var categoryNames = userCategories.Select(c => c.Name).ToList();

        // 2. Extact Data from the Image via Vision API
        var extractedData = await _visionProvider.ExtractReceiptDataAsync(
            imageStream, 
            mimeType, 
            categoryNames, 
            cancellationToken);

        // 3. Map to Invoice
        var invoice = new Invoice
        {
            UserId = userId,
            MerchantName = extractedData.Invoice.MerchantName ?? extractedData.Expense.Merchant,
            Date = extractedData.Invoice.Date ?? extractedData.Expense.Date,
            TotalAmount = extractedData.Invoice.TotalAmount > 0 
                ? extractedData.Invoice.TotalAmount 
                : extractedData.Expense.TotalAmount,
            Status = "Pending"
        };

        // Map to Expense
        var expense = new Expense
        {
            UserId = userId,
            Merchant = extractedData.Expense.Merchant ?? invoice.MerchantName,
            InvoiceNumber = extractedData.Expense.InvoiceNumber ?? extractedData.Invoice.InvoiceNumber,
            Date = extractedData.Expense.Date ?? invoice.Date ?? DateTime.UtcNow,
            TotalAmount = extractedData.Expense.TotalAmount > 0 
                ? extractedData.Expense.TotalAmount 
                : invoice.TotalAmount,
            Status = ExpenseStatus.NeedsReview, // Using hardcoded value
            AiExtractionStatus = AiExtractionStatus.Completed,
            Invoice = invoice // Link Expense to Invoice
        };

        // Cache category lookup dictionary for optimal assignment
        var categoryDict = userCategories.ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

        // Resolve Overall Category ID for the Expense
        int overallCategoryId = 0;
        if (!string.IsNullOrWhiteSpace(extractedData.Expense.OverallCategory) && 
            categoryDict.TryGetValue(extractedData.Expense.OverallCategory, out var oId))
        {
            overallCategoryId = oId;
        }

        // 4. Map Line Items
        foreach (var extractedItem in extractedData.LineItems)
        {
            int categoryId = overallCategoryId; // Default to overall categorization
            if (!string.IsNullOrWhiteSpace(extractedItem.Category) && 
                categoryDict.TryGetValue(extractedItem.Category, out var id))
            {
                categoryId = id;
            }
            
            // If still zero, fallback to a default or first available just for robustness 
            if (categoryId == 0)
            {
                categoryId = userCategories.FirstOrDefault()?.Id ?? 0;
            }

            // Link Items to Expense
            var expenseItem = new ExpenseItem
            {
                Description = extractedItem.Description,
                ItemAmount = extractedItem.TotalPrice > 0 ? extractedItem.TotalPrice : (extractedItem.Quantity * extractedItem.UnitPrice),
                CategoryId = categoryId
            };
            expense.Items.Add(expenseItem);

            // Also Link Items to Invoice to maintain integrity across the domain
            var invoiceItem = new InvoiceItem
            {
                ProductName = extractedItem.Description,
                Quantity = extractedItem.Quantity > 0 ? extractedItem.Quantity : 1,
                UnitPrice = extractedItem.UnitPrice,
                TotalPrice = extractedItem.TotalPrice,
                AssignedCategoryId = categoryId
            };
            invoice.Items.Add(invoiceItem);
        }

        // 5. Registration and Save methods
        await _expenseRepository.AddAsync(expense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return expense;
    }
}
