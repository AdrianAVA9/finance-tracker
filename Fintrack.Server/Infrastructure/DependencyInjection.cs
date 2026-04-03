using System.Runtime.CompilerServices;
using Fintrack.Server.Data;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
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

            // --- OCR / Vision Integrations ---
            services.AddHttpClient<Fintrack.Server.Application.Abstractions.Vision.IVisionExtractionProvider, Fintrack.Server.Infrastructure.Vision.GeminiVisionExtractionProvider>();
            services.AddScoped<Fintrack.Server.Application.Expenses.ReceiptProcessingService>();
            
            // NOTE: Make sure IExpenseRepository is registered if it's not already!
            services.AddScoped<IExpenseRepository, ExpenseRepository>();

            services.AddHostedService<RecurringTransactionProcessorJob>();
            services.AddHostedService<RecurringBudgetProcessorJob>();

            return services;
        }
    }
}
