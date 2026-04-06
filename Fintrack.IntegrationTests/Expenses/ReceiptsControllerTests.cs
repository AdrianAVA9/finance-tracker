using System.Net;
using System.Net.Http.Headers;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Application.DTOs.Vision;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fintrack.IntegrationTests.Expenses;

public class ReceiptsControllerTests : BaseIntegrationTest
{
    public ReceiptsControllerTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ProcessReceipt_Should_ReturnOk_When_ValidImageIsUploaded()
    {
        // Arrange — use shared factory mock so this host uses the same SQLite DB as InitializeAsync
        var mockVision = Factory.VisionExtractionProviderMock;
        var extractedData = new ExtractedReceiptData
        {
            Invoice = new ExtractedInvoiceData
            {
                MerchantName = "Test Integration Merchant",
                TotalAmount = 50.0m,
                InvoiceNumber = "INT-999"
            },
            Expense = new ExtractedExpenseData
            {
                OverallCategory = "Supermercado",
                Merchant = "Test Integration Merchant",
                TotalAmount = 50.0m
            },
            LineItems =
            [
                new ExtractedLineItemData
                {
                    Description = "Item A",
                    Quantity = 1,
                    UnitPrice = 50.0m,
                    TotalPrice = 50.0m,
                    Category = "Supermercado"
                }
            ]
        };

        mockVision
            .ExtractReceiptDataAsync(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(extractedData);

        var userId = Guid.NewGuid().ToString();
        Client.DefaultRequestHeaders.Remove(TestAuthHandler.TestUserIdHeader);
        Client.DefaultRequestHeaders.Add(TestAuthHandler.TestUserIdHeader, userId);

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "file", "receipt.jpg");

        // Act
        var response = await PostHttpContentAsync("/api/v1/receipts/process", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ProcessReceipt_Should_ReturnBadRequest_When_NoFileIsUploaded()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId);

        using var content = new MultipartFormDataContent();

        var response = await PostHttpContentAsync("/api/v1/receipts/process", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
