using System.Runtime.CompilerServices;
using Fintrack.Server.Application.Abstractions.Clock;
using Fintrack.Server.Application.Abstractions.Storage;
using Fintrack.Server.Application.Abstractions.Vision;
using Fintrack.Server.Application.Budgets;
using Fintrack.Server.Application.Expenses;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Infrastructure.BackgroundJobs;
using Fintrack.Server.Infrastructure.Clock;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Infrastructure.Repositories;
using Fintrack.Server.Infrastructure.Storage;
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
        services.AddScoped<IIncomeCategoryRepository, IncomeCategoryRepository>();
        services.AddScoped<IExpenseCategoryGroupRepository, ExpenseCategoryGroupRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IRecurringIncomeRepository, RecurringIncomeRepository>();

        // --- OCR / Vision Integrations ---
        services.AddHttpClient<IVisionExtractionProvider, GeminiVisionExtractionProvider>();
        services.AddScoped<ReceiptProcessingService>();

        // NOTE: Make sure IExpenseRepository is registered if it's not already!
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPendingReceiptExtractionJobRepository, PendingReceiptExtractionJobRepository>();
        services.AddSingleton<IReceiptPendingFileStore, LocalReceiptPendingFileStore>();
        services.AddScoped<PendingReceiptExtractionProcessor>();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<IRecurringBudgetRollForwardService, RecurringBudgetRollForwardService>();

        services.AddHostedService<RecurringTransactionProcessorJob>();
        services.AddHostedService<RecurringBudgetProcessorJob>();
        services.AddHostedService<PendingReceiptExtractionBackgroundService>();

        return services;
    }
}
