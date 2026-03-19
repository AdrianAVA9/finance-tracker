using System.Runtime.CompilerServices;
using Fintrack.Server.Data;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Infrastructure.BackgroundJobs;
using Fintrack.Server.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Fintrack.Tests")]

namespace Fintrack.Server.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
            services.AddScoped<IExpenseCategoryGroupRepository, ExpenseCategoryGroupRepository>();

            services.AddHostedService<RecurringTransactionProcessorJob>();

            return services;
        }
    }
}
