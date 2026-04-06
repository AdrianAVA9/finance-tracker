using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Api.Controllers.Expenses;
using FluentAssertions;
using Xunit;

namespace Fintrack.IntegrationTests.Expenses;

public class CreateExpenseTests : BaseIntegrationTest
{
    public CreateExpenseTests(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task Create_Should_ReturnCreated_When_ValidSimpleExpense()
    {
        // Arrange
        AuthenticateAs(Guid.NewGuid().ToString());

        var request = new RequestCreateExpense
        {
            UserId = Guid.NewGuid().ToString(),
            Merchant = "Integration Merchant",
            TotalAmount = 100.00m,
            Date = DateTime.UtcNow,
            IsRecurring = false,
            Items = new List<RequestExpenseItemData>
            {
                new RequestExpenseItemData(Guid.NewGuid(), 100.00m, "Test note") // Match simple expense logic
            }
        };

        // Act
        var response = await PostAsync("/api/v1/expenses", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_MathMismatchInSplitExpense()
    {
        // Arrange
        AuthenticateAs(Guid.NewGuid().ToString());

        var request = new RequestCreateExpense
        {
            UserId = Guid.NewGuid().ToString(),
            Merchant = "Integration Merchant Split",
            TotalAmount = 100.00m,
            Date = DateTime.UtcNow,
            IsRecurring = false,
            Items = new List<RequestExpenseItemData>
            {
                new RequestExpenseItemData(Guid.NewGuid(), 50.00m, "Item 1"),
                new RequestExpenseItemData(Guid.NewGuid(), 30.00m, "Item 2")
                // Total is 80, but stated is 100
            }
        };

        // Act
        var response = await PostAsync("/api/v1/expenses", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("sum of items");
    }
}
