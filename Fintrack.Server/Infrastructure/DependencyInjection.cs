using System.Runtime.CompilerServices;
using Fintrack.Server.Application.Abstractions.Vision;
using Fintrack.Server.Application.Expenses;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Infrastructure.BackgroundJobs;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Infrastructure.Repositories;
using Fintrack.Server.Infrastructure.Vision;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Fintrack.Tests")]

namespace Fintrack.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
        services.AddScoped<IExpenseCategoryGroupRepository, ExpenseCategoryGroupRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<IRecurringIncomeRepository, RecurringIncomeRepository>();

        // --- OCR / Vision Integrations ---
        services.AddHttpClient<IVisionExtractionProvider, GeminiVisionExtractionProvider>();
        services.AddScoped<ReceiptProcessingService>();

        // NOTE: Make sure IExpenseRepository is registered if it's not already!
        services.AddScoped<IExpenseRepository, ExpenseRepository>();

        services.AddHostedService<RecurringTransactionProcessorJob>();
        services.AddHostedService<RecurringBudgetProcessorJob>();

        return services;
    }
}
