using System.Reflection;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Tests.TestData.Budgets;

/// <summary>
/// Consistent sample data and helpers for budget-related unit tests.
/// <see cref="Budget.Category"/> has a private setter; tests use reflection so handlers can be exercised without EF.
/// </summary>
internal static class BudgetTestDoubles
{
    public const string DefaultUserId = "user-1";

    public static ExpenseCategoryGroup CreateGroup(int id = 1, string name = "Vivienda") =>
        new()
        {
            Id = id,
            Name = name
        };

    public static ExpenseCategory CreateCategory(
        int id = 1,
        string name = "Alquiler",
        ExpenseCategoryGroup? group = null,
        string? color = "#FF0000",
        string? icon = "home")
    {
        group ??= CreateGroup();
        return new ExpenseCategory
        {
            Id = id,
            Name = name,
            GroupId = group.Id,
            Group = group,
            Color = color,
            Icon = icon
        };
    }

    public static Budget CreateBudget(
        string userId = DefaultUserId,
        int categoryId = 1,
        decimal amount = 1000m,
        bool isRecurrent = false,
        int month = 3,
        int year = 2024)
    {
        var result = Budget.Create(userId, categoryId, amount, isRecurrent, month, year);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error.Description);
        }

        return result.Value;
    }

    /// <summary>
    /// Returns a budget with <see cref="Budget.Category"/> populated, as <see cref="Fintrack.Server.Application.Budgets.Queries.GetBudgets.GetBudgetsQueryHandler"/> expects.
    /// </summary>
    public static Budget CreateBudgetWithCategory(
        ExpenseCategory? category = null,
        string userId = DefaultUserId,
        decimal amount = 1000m,
        bool isRecurrent = false,
        int month = 3,
        int year = 2024)
    {
        category ??= CreateCategory();
        var budget = CreateBudget(userId, category.Id, amount, isRecurrent, month, year);
        AttachCategory(budget, category);
        return budget;
    }

    private static void AttachCategory(Budget budget, ExpenseCategory category)
    {
        var prop = typeof(Budget).GetProperty(nameof(Budget.Category))
                   ?? throw new InvalidOperationException("Budget.Category not found");
        var setter = prop.GetSetMethod(nonPublic: true)
                     ?? throw new InvalidOperationException("Budget.Category has no setter");
        setter.Invoke(budget, new object?[] { category });
    }
}
