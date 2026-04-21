using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Api.Controllers.Incomes;
using Fintrack.Server.Application.Incomes.Queries.GetIncomeById;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fintrack.IntegrationTests.Incomes;

public class IncomesControllerTests : BaseIntegrationTest
{
    private static readonly string[] IncomeUserPermissions =
    {
        Permissions.IncomesRead,
        Permissions.IncomesWrite,
        Permissions.IncomesDelete
    };

    public IncomesControllerTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    private async Task<Guid> GetAnySeededIncomeCategoryIdAsync()
    {
        return await DbContext.IncomeCategories
            .Select(c => c.Id)
            .FirstAsync();
    }

    [Fact]
    public async Task Create_Should_ReturnCreated_When_Valid()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, IncomeUserPermissions);

        var categoryId = await GetAnySeededIncomeCategoryIdAsync();
        var request = new CreateIncomeRequest(
            Source: "Integration salary",
            Amount: 250m,
            CategoryId: categoryId,
            Date: DateTime.UtcNow.Date,
            Notes: null,
            IsRecurring: false,
            Frequency: null,
            NextDate: null);

        var response = await PostAsync("/api/v1/incomes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var newId = await response.Content.ReadFromJsonAsync<Guid>();
        newId.Should().NotBeEmpty();

        DbContext.ChangeTracker.Clear();
        var persisted = await DbContext.Incomes.SingleOrDefaultAsync(i => i.Id == newId);
        persisted.Should().NotBeNull();
        persisted!.Source.Should().Be("Integration salary");
        persisted.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetById_Should_ReturnOk_When_IncomeExists()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, IncomeUserPermissions);

        var categoryId = await GetAnySeededIncomeCategoryIdAsync();
        var createRequest = new CreateIncomeRequest(
            "Job",
            75m,
            categoryId,
            DateTime.UtcNow.Date,
            null,
            false,
            null,
            null);

        var createResponse = await PostAsync("/api/v1/incomes", createRequest);
        var incomeId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await GetAsync($"/api/v1/incomes/{incomeId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<IncomeDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(incomeId);
        dto.Source.Should().Be("Job");
    }

    [Fact]
    public async Task Update_Should_ReturnOk_When_IncomeExists()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, IncomeUserPermissions);

        var categoryId = await GetAnySeededIncomeCategoryIdAsync();
        var createResponse = await PostAsync(
            "/api/v1/incomes",
            new CreateIncomeRequest(
                "Old",
                10m,
                categoryId,
                DateTime.UtcNow.Date,
                null,
                false,
                null,
                null));
        var incomeId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var updateResponse = await PutAsync(
            $"/api/v1/incomes/{incomeId}",
            new UpdateIncomeRequest(
                "New",
                20m,
                categoryId,
                DateTime.UtcNow.Date,
                "x",
                false,
                null,
                null));

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var updated = await DbContext.Incomes.FindAsync(incomeId);
        updated!.Source.Should().Be("New");
        updated.Amount.Should().Be(20m);
    }

    [Fact]
    public async Task Delete_Should_RemoveIncome()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, IncomeUserPermissions);

        var categoryId = await GetAnySeededIncomeCategoryIdAsync();
        var createResponse = await PostAsync(
            "/api/v1/incomes",
            new CreateIncomeRequest(
                "To delete",
                5m,
                categoryId,
                DateTime.UtcNow.Date,
                null,
                false,
                null,
                null));
        var incomeId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var deleteResponse = await DeleteAsync($"/api/v1/incomes/{incomeId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        (await DbContext.Incomes.FindAsync(incomeId)).Should().BeNull();
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_OtherUser()
    {
        var ownerId = Guid.NewGuid().ToString();
        AuthenticateAs(ownerId, IncomeUserPermissions);

        var categoryId = await GetAnySeededIncomeCategoryIdAsync();
        var createResponse = await PostAsync(
            "/api/v1/incomes",
            new CreateIncomeRequest(
                "Private",
                1m,
                categoryId,
                DateTime.UtcNow.Date,
                null,
                false,
                null,
                null));
        var incomeId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        RemoveAuthentication();
        AuthenticateAs(Guid.NewGuid().ToString(), IncomeUserPermissions);

        var response = await GetAsync($"/api/v1/incomes/{incomeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
