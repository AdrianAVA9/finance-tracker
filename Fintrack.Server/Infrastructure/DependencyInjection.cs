using System.Runtime.CompilerServices;
using Fintrack.Server.Data;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Infrastructure.Repositories;

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

            return services;
        }
    }
}
