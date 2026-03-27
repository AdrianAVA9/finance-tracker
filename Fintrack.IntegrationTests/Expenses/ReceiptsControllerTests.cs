using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Application.Abstractions.Vision;
using Fintrack.Server.Application.DTOs.Vision;
using Fintrack.Server.Models.Enums;

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
        // Arrange
        var mockVisionProvider = Substitute.For<IVisionExtractionProvider>();
        
        var extractedData = new ExtractedReceiptData
        {
            Invoice = new ExtractedInvoiceData { MerchantName = "Test Integration Merchant", TotalAmount = 50.0m, InvoiceNumber = "INT-999" },
            Expense = new ExtractedExpenseData { OverallCategory = "General" }
        };

        mockVisionProvider.ExtractReceiptDataAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(extractedData);

        // We create a custom client that overrides the VisionProvider for this specific test
        var customClient = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Remove existing if any, and inject our mock
                services.AddSingleton<IVisionExtractionProvider>(mockVisionProvider);
            });
        }).CreateClient();

        var userId = Guid.NewGuid().ToString();
        customClient.DefaultRequestHeaders.Add(TestAuthHandler.TestUserIdHeader, userId);

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "file", "receipt.jpg");

        // Act
        var response = await customClient.PostAsync("/api/v1/receipts/process", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task ProcessReceipt_Should_ReturnBadRequest_When_NoFileIsUploaded()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId); // Uses base class helper on the default Client

        // Sending empty multipart or just no "file" parameter will trigger BadRequest
        using var content = new MultipartFormDataContent();
        
        // Act
        var response = await PostAsync("/api/v1/receipts/process", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
