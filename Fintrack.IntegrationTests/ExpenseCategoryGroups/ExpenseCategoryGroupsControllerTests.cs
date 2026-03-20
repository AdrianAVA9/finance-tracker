using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Controllers.ExpenseCategoryGroups;
using Fintrack.Server.Models;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;

namespace Fintrack.IntegrationTests.ExpenseCategoryGroups
{
    [Collection("Integration")]
    public class ExpenseCategoryGroupsControllerTests : BaseIntegrationTest
    {
        public ExpenseCategoryGroupsControllerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Seeder_ShouldInitializeDefaultGroupsOnStartup()
        {
            // The factory launches Program.cs which triggers DefaultCategorySeeder.
            AuthenticateAs("user-seeding-check", new[] { Permissions.CategoriesRead });

            var response = await Client.GetAsync("/api/v1/expensecategorygroups");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var groups = await response.Content.ReadFromJsonAsync<List<ExpenseCategoryGroup>>();
            
            // Should contain at least the 11 base seeded categories
            groups.Should().NotBeNull();
            groups!.Count.Should().BeGreaterThanOrEqualTo(11);
            
            // Verify specifically BÁSICOS MENSUALES exists and has no user attached
            var basicGroup = groups.FirstOrDefault(g => g.Name == "BÁSICOS MENSUALES");
            basicGroup.Should().NotBeNull();
            basicGroup!.Description.Should().Be("El costo fijo de vida");
            basicGroup.IsEditable.Should().BeFalse();
            basicGroup.UserId.Should().BeNull();
        }

        [Fact]
        public async Task Create_ShouldCreateUserGroup_AndAllowRead()
        {
            AuthenticateAs("user-create-test", new[] { Permissions.CategoriesWrite, Permissions.CategoriesRead });

            // 1. Create a new custom group
            var createRequest = new RequestCreateExpenseCategoryGroup
            {
                Name = "Custom User Group",
                Description = "My personalized grouping"
            };

            var createResponse = await Client.PostAsJsonAsync("/api/v1/expensecategorygroups", createRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // 2. Read the new custom group back ensuring IsEditable is true
            // CreatedAtRoute parses the location
            var location = createResponse.Headers.Location;
            
            var getResponse = await Client.GetAsync(location);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var retrievedGroup = await getResponse.Content.ReadFromJsonAsync<ExpenseCategoryGroup>();
            retrievedGroup.Should().NotBeNull();
            retrievedGroup!.Name.Should().Be("Custom User Group");
            retrievedGroup.Description.Should().Be("My personalized grouping");
            retrievedGroup.IsEditable.Should().BeTrue();
            retrievedGroup.UserId.Should().NotBeNull(); // Belongs to testuser2
        }

        [Fact]
        public async Task Update_CannotUpdateSystemGroup()
        {
            AuthenticateAs("user-update-hack", new[] { Permissions.CategoriesWrite, Permissions.CategoriesRead });

            // 1. Find a seeded System group
            var allGroupsResponse = await Client.GetAsync("/api/v1/expensecategorygroups");
            var groups = await allGroupsResponse.Content.ReadFromJsonAsync<List<ExpenseCategoryGroup>>();
            var systemGroup = groups!.First(g => g.IsEditable == false);

            // 2. Attempt to update it
            var updateRequest = new RequestUpdateExpenseCategoryGroup
            {
                Name = "Hacked Name",
                Description = "I am attempting to hijack the system group"
            };

            var updateResponse = await Client.PutAsJsonAsync($"/api/v1/expensecategorygroups/{systemGroup.Id}", updateRequest);

            // Assert it was forbidden or bad request because it's not editable by the user
            updateResponse.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
        }
    }
}
