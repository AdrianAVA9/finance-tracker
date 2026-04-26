using System.Linq;
using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Api.Controllers.Incomes;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fintrack.IntegrationTests.IncomeCategories;

[Collection("Integration")]
public sealed class IncomeCategoriesIntegrationTests : BaseIntegrationTest
{
    public IncomeCategoriesIntegrationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetCategories_Should_Return401_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await GetAsync("/api/v1/incomes/categories");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCategories_Should_ReturnSeededCategories_When_Authenticated()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, new[] { Permissions.IncomesRead });

        var response = await GetAsync("/api/v1/incomes/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<IncomeCategoryResponse>>();
        list.Should().NotBeNull();
        list!.Should().NotBeEmpty();
        list.Should().Contain(c => c.Name == "Salario / Nómina" && c.IsEditable == false);
    }

    [Fact]
    public async Task GetUserOwnedCategories_Should_Return401_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await GetAsync("/api/v1/incomes/categories/owned");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserOwnedCategories_Should_ReturnEmpty_When_OnlySeededGlobalCategoriesExist()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, new[] { Permissions.IncomesRead });

        var response = await GetAsync("/api/v1/incomes/categories/owned");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<IncomeCategoryResponse>>();
        list.Should().NotBeNull();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserOwnedCategories_Should_ReturnPersistedUserCategory()
    {
        var userId = Guid.NewGuid().ToString();
        var create = IncomeCategory.Create("Custom user income", "work", "#112233", userId, isEditable: true);
        create.IsSuccess.Should().BeTrue();
        DbContext.IncomeCategories.Add(create.Value);
        await DbContext.SaveChangesAsync();

        AuthenticateAs(userId, new[] { Permissions.IncomesRead });

        var response = await GetAsync("/api/v1/incomes/categories/owned");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<IncomeCategoryResponse>>();
        list.Should().NotBeNull();
        list!.Should().Contain(c => c.Name == "Custom user income" && c.IsEditable);
    }

    [Fact]
    public async Task CreateCategory_Should_Return201_And_Persist()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, new[] { Permissions.IncomesRead, Permissions.IncomesWrite });

        var request = new RequestCreateIncomeCategory("Consultoría", "work", "#05E699");
        var response = await PostAsync("/api/v1/incomes/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var newId = await response.Content.ReadFromJsonAsync<Guid>();
        newId.Should().NotBeEmpty();

        var fromDb = await DbContext.IncomeCategories.FindAsync(newId);
        fromDb.Should().NotBeNull();
        fromDb!.Name.Should().Be("Consultoría");
        fromDb.UserId.Should().Be(userId);
        fromDb.IsEditable.Should().BeTrue();
    }

    [Fact]
    public async Task GetCategoryById_Should_ReturnCategory_When_UserOwnsIt()
    {
        var userId = Guid.NewGuid().ToString();
        var create = IncomeCategory.Create("Mío", "savings", "#abc", userId, isEditable: true);
        create.IsSuccess.Should().BeTrue();
        DbContext.IncomeCategories.Add(create.Value);
        await DbContext.SaveChangesAsync();
        var id = create.Value.Id;

        AuthenticateAs(userId, new[] { Permissions.IncomesRead });

        var response = await GetAsync($"/api/v1/incomes/categories/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<IncomeCategoryResponse>();
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("Mío");
        dto.IsEditable.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateCategory_Should_Succeed_For_OwnedEditableCategory()
    {
        var userId = Guid.NewGuid().ToString();
        var create = IncomeCategory.Create("Old", "icon", null, userId, isEditable: true);
        create.IsSuccess.Should().BeTrue();
        DbContext.IncomeCategories.Add(create.Value);
        await DbContext.SaveChangesAsync();
        var id = create.Value.Id;

        AuthenticateAs(userId, new[] { Permissions.IncomesWrite });

        var request = new RequestUpdateIncomeCategory("Nuevo", "savings", "#112233");
        var response = await PutAsync($"/api/v1/incomes/categories/{id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        DbContext.ChangeTracker.Clear();
        var row = await DbContext.IncomeCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        row.Should().NotBeNull();
        row!.Name.Should().Be("Nuevo");
    }

    [Fact]
    public async Task UpdateCategory_Should_Return400_When_SystemCategoryNotEditable()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, new[] { Permissions.IncomesRead, Permissions.IncomesWrite });

        var listResp = await GetAsync("/api/v1/incomes/categories");
        var list = await listResp.Content.ReadFromJsonAsync<List<IncomeCategoryResponse>>();
        var system = list!.First(c => c.Name == "Salario / Nómina" && c.IsEditable == false);

        var request = new RequestUpdateIncomeCategory("Hacked", null, null);
        var response = await PutAsync($"/api/v1/incomes/categories/{system.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public sealed class IncomeCategoryResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Icon { get; init; }
        public string? Color { get; init; }
        public bool IsEditable { get; init; }
    }
}
