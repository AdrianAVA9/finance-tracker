using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Fintrack.Server.Application.Abstractions.Vision;
using Fintrack.Server.Application.DTOs.Vision;
using Fintrack.Server.Application.Expenses;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Models;
using Fintrack.Server.Models.Enums;

namespace Fintrack.Tests.Application.Expenses;

public class ReceiptProcessingServiceTests
{
    private readonly IVisionExtractionProvider _visionProvider;
    private readonly IExpenseCategoryRepository _categoryRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ReceiptProcessingService _service;

    public ReceiptProcessingServiceTests()
    {
        _visionProvider = Substitute.For<IVisionExtractionProvider>();
        _categoryRepository = Substitute.For<IExpenseCategoryRepository>();
        _expenseRepository = Substitute.For<IExpenseRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _service = new ReceiptProcessingService(
            _visionProvider,
            _categoryRepository,
            _expenseRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task ProcessReceiptAsync_Should_ReturnMappedExpense_When_ValidImageIsProvided()
    {
        // Arrange
        var userId = "test-user-id";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake-image-content"));
        var mimeType = "image/jpeg";

        var categories = new List<ExpenseCategory>
        {
            new ExpenseCategory { Id = 1, Name = "Food" },
            new ExpenseCategory { Id = 2, Name = "Transport" }
        };

        _categoryRepository.GetAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);

        var extractedData = new ExtractedReceiptData
        {
            Invoice = new ExtractedInvoiceData { MerchantName = "TestMerchant", TotalAmount = 15.5m, InvoiceNumber = "INV-2026" },
            Expense = new ExtractedExpenseData { OverallCategory = "Food" },
            LineItems = new List<ExtractedLineItemData>
            {
                new ExtractedLineItemData { Description = "Burger", TotalPrice = 15.5m, Category = "Food", Quantity = 1, UnitPrice = 15.5m }
            }
        };

        _visionProvider.ExtractReceiptDataAsync(stream, mimeType, Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(extractedData);

        // Act
        var expense = await _service.ProcessReceiptAsync(stream, mimeType, userId);

        // Assert
        expense.Should().NotBeNull();
        expense.UserId.Should().Be(userId);
        expense.Merchant.Should().Be("TestMerchant");
        expense.InvoiceNumber.Should().Be("INV-2026");
        expense.Status.Should().Be(ExpenseStatus.NeedsReview);
        expense.TotalAmount.Should().Be(15.5m);
        expense.Items.Should().ContainSingle();
        expense.Items.First().CategoryId.Should().Be(1);

        await _expenseRepository.Received(1).AddAsync(Arg.Any<Expense>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessReceiptAsync_Should_FallbackToFirstCategory_When_UnknownCategoryProvided()
    {
         // Arrange
        var userId = "test-user-id";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake-image-content"));
        var mimeType = "image/jpeg";

        var categories = new List<ExpenseCategory>
        {
            new ExpenseCategory { Id = 99, Name = "DefaultFallback" }
        };

        _categoryRepository.GetAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);

        var extractedData = new ExtractedReceiptData
        {
            Invoice = new ExtractedInvoiceData { MerchantName = "UnknownStore", TotalAmount = 5.0m },
            Expense = new ExtractedExpenseData { OverallCategory = "TotallyUnknownCategory" },
            LineItems = new List<ExtractedLineItemData>()
        };

        _visionProvider.ExtractReceiptDataAsync(stream, mimeType, Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(extractedData);

        // Act
        var expense = await _service.ProcessReceiptAsync(stream, mimeType, userId);

        // Assert
        expense.Should().NotBeNull();
        
        // Since no line items were extracted, the logic adds 0 items. Let's provide a line item to test fallback logic.
        extractedData.LineItems.Add(new ExtractedLineItemData { Description = "UnknownItem", Category = "AlsoUnknown", TotalPrice = 5m });
        
        var expenseWithItem = await _service.ProcessReceiptAsync(stream, mimeType, userId);
        
        expenseWithItem.Items.Should().ContainSingle();
        expenseWithItem.Items.First().CategoryId.Should().Be(99); // Fallbacks to first available
    }
}
