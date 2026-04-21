using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Infrastructure.Data.Seeders;
using Xunit;

namespace Fintrack.IntegrationTests.Infrastructure
{
    [Collection("Integration")]
    public abstract class BaseIntegrationTest : IAsyncLifetime
    {
        protected readonly IntegrationTestWebAppFactory Factory;
        protected readonly HttpClient Client;
        protected readonly IServiceScope Scope;
        protected readonly ApplicationDbContext DbContext;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
            Scope = factory.Services.CreateScope();
            DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        protected void AuthenticateAs(
            string userId,
            string[]? permissions = null,
            string[]? roles = null)
        {
            Client.DefaultRequestHeaders.Add(TestAuthHandler.TestUserIdHeader, userId);

            if (roles?.Length > 0)
            {
                Client.DefaultRequestHeaders.Add(
                    TestAuthHandler.TestUserRolesHeader,
                    string.Join(",", roles));
            }

            if (permissions?.Length > 0)
            {
                Client.DefaultRequestHeaders.Add(
                    TestAuthHandler.TestUserPermissionsHeader,
                    string.Join(",", permissions));
            }
        }

        protected void RemoveAuthentication()
        {
            Client.DefaultRequestHeaders.Remove(TestAuthHandler.TestUserIdHeader);
            Client.DefaultRequestHeaders.Remove(TestAuthHandler.TestUserPermissionsHeader);
            Client.DefaultRequestHeaders.Remove(TestAuthHandler.TestUserRolesHeader);
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T content)
        {
            return await Client.PostAsJsonAsync(url, content);
        }

        /// <summary>
        /// Use for multipart and other non-JSON bodies. Do not use <see cref="PostAsync{T}"/> with <see cref="HttpContent"/> (that sends JSON).
        /// </summary>
        protected async Task<HttpResponseMessage> PostHttpContentAsync(string url, HttpContent content)
        {
            return await Client.PostAsync(url, content);
        }

        protected async Task<HttpResponseMessage> PutAsync<T>(string url, T content)
        {
            return await Client.PutAsJsonAsync(url, content);
        }

        protected async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await Client.GetAsync(url);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await Client.DeleteAsync(url);
        }

        protected async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            DbContext.Set<TEntity>().Add(entity);
            await DbContext.SaveChangesAsync();
        }

        protected async Task<TEntity?> FindAsync<TEntity>(int id) where TEntity : class
        {
            return await DbContext.Set<TEntity>().FindAsync(id);
        }

        protected async Task<TEntity?> FindAsync<TEntity>(Guid id) where TEntity : class
        {
            return await DbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task InitializeAsync()
        {
            // Re-create the database for each test to ensure test isolation
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.Database.EnsureCreatedAsync();
            
            // Re-run the startup seeder for the fresh database
            await DefaultCategorySeeder.SeedAsync(DbContext);
        }

        public Task DisposeAsync()
        {
            Scope.Dispose();
            return Task.CompletedTask;
        }
    }

    [CollectionDefinition("Integration")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>
    {
    }
}
