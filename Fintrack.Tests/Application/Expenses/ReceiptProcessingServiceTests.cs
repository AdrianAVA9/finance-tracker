using System.Text;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Fintrack.Server.Application.Abstractions.Vision;
using Fintrack.Server.Application.DTOs.Vision;
using Fintrack.Server.Application.Expenses;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;

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

        var food = ExpenseCategory.Create("Food", null, null, null, null, null, true).Value;
        var transport = ExpenseCategory.Create("Transport", null, null, null, null, null, true).Value;
        var categories = new List<ExpenseCategory> { food, transport };

        _categoryRepository.GetAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);

        var extractedData = new ExtractedReceiptData
        {
            Invoice = new ExtractedInvoiceData { MerchantName = "TestMerchant", TotalAmount = 15.5m, InvoiceNumber = "INV-2026" },
            Expense = new ExtractedExpenseData { OverallCategory = "Food", TotalAmount = 15.5m },
            LineItems = new List<ExtractedLineItemData>
            {
                new() { Description = "Burger", TotalPrice = 15.5m, Category = "Food", Quantity = 1, UnitPrice = 15.5m }
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
        expense.Status.Should().Be(ExpenseStatus.NeedsReview);
        expense.TotalAmount.Should().Be(15.5m);
        expense.Items.Should().ContainSingle();
        expense.Items.First().CategoryId.Should().Be(food.Id);

        _expenseRepository.Received(1).Add(Arg.Any<Expense>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessReceiptAsync_Should_FallbackToFirstCategory_When_UnknownCategoryProvided()
    {
        // Arrange
        var userId = "test-user-id";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake-image-content"));
        var mimeType = "image/jpeg";

        var fallback = ExpenseCategory.Create("DefaultFallback", null, null, null, null, null, true).Value;
        var categories = new List<ExpenseCategory> { fallback };

        _categoryRepository.GetAllByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);

        var extractedData = new ExtractedReceiptData
        {
            Invoice = new ExtractedInvoiceData { MerchantName = "UnknownStore", TotalAmount = 5.0m },
            Expense = new ExtractedExpenseData { OverallCategory = "TotallyUnknownCategory", TotalAmount = 5.0m },
            LineItems = new List<ExtractedLineItemData>
            {
                new() { Description = "UnknownItem", Category = "AlsoUnknown", TotalPrice = 5m, Quantity = 1, UnitPrice = 5m }
            }
        };

        _visionProvider.ExtractReceiptDataAsync(stream, mimeType, Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(extractedData);

        // Act
        var expense = await _service.ProcessReceiptAsync(stream, mimeType, userId);

        // Assert
        expense.Should().NotBeNull();
        expense.Items.Should().ContainSingle();
        expense.Items.First().CategoryId.Should().Be(fallback.Id);
    }
}
