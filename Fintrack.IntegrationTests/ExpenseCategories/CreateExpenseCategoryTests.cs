using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Fintrack.Server.Api.Controllers.ExpenseCategories;
using Fintrack.IntegrationTests.Infrastructure;
using Xunit;

namespace Fintrack.IntegrationTests.ExpenseCategories
{
    public class CreateExpenseCategoryTests : BaseIntegrationTest
    {
        public CreateExpenseCategoryTests(IntegrationTestWebAppFactory factory) 
            : base(factory)
        {
        }

        [Fact]
        public async Task Create_Should_ReturnCreated_When_ValidRequest()
        {
            // Arrange
            var testUserId = "user-12345";
            AuthenticateAs(testUserId, new[] { "categories:write" });

            var request = new RequestCreateExpenseCategory
            {
                Name = "Groceries",
                Icon = "shopping-cart",
                Color = "#FF0000"
            };

            // Act
            var response = await PostAsync("/api/v1/expensecategories", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var entityId = await response.Content.ReadFromJsonAsync<int>();
            entityId.Should().BeGreaterThan(0);

            // Verify in database
            var entity = await DbContext.ExpenseCategories.FindAsync(entityId);
            entity.Should().NotBeNull();
            entity!.Name.Should().Be(request.Name);
            entity.UserId.Should().Be(testUserId);
        }

        [Fact]
        public async Task Create_Should_ReturnUnauthorized_When_NotAuthenticated()
        {
            // Arrange
            RemoveAuthentication();

            var request = new RequestCreateExpenseCategory
            {
                Name = "Groceries",
                Icon = "shopping-cart",
                Color = "#FF0000"
            };

            // Act
            var response = await PostAsync("/api/v1/expensecategories", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_Should_ReturnForbidden_When_NoWritePermission()
        {
            // Arrange
            AuthenticateAs("user-12345", new[] { "categories:read" }); // Only read permission

            var request = new RequestCreateExpenseCategory
            {
                Name = "Groceries",
                Icon = "shopping-cart",
                Color = "#FF0000"
            };

            // Act
            var response = await PostAsync("/api/v1/expensecategories", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
