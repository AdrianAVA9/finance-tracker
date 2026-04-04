using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using NSubstitute;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Fintrack.IntegrationTests.Infrastructure
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection? _connection;

        /// <summary>
        /// Exposes the shared mock email sender so tests can verify calls.
        /// </summary>
        public IEmailSender<ApplicationUser> MockEmailSender { get; } =
            Substitute.For<IEmailSender<ApplicationUser>>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                services.RemoveAll(typeof(ApplicationDbContext));
                services.RemoveAll(typeof(DbConnection));

                // Create open SqliteConnection so in-memory database isn't destroyed
                _connection = new SqliteConnection("DataSource=:memory:;Foreign Keys=False;");
                _connection.Open();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<TestAuthSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    options => { });

                // Replace the real email sender with a mock so tests can verify dispatch
                // without requiring a real SMTP server.
                services.RemoveAll(typeof(IEmailSender<ApplicationUser>));
                services.AddSingleton(MockEmailSender);
            });

            builder.UseEnvironment("Testing");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Dispose();
        }
    }
}
