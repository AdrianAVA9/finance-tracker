using System.Reflection;
using Fintrack.Server.Domain.Abstractions;
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

    public static ExpenseCategoryGroup CreateGroup(Guid? id = null, string name = "Vivienda")
    {
        var result = ExpenseCategoryGroup.CreateForSystem(name, null);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error.Description);
        }

        var group = result.Value;
        if (id.HasValue)
        {
            SetEntityId(group, id.Value);
        }

        return group;
    }

    public static ExpenseCategory CreateCategory(
        Guid? id = null,
        string name = "Alquiler",
        ExpenseCategoryGroup? group = null,
        string? color = "#FF0000",
        string? icon = "home")
    {
        group ??= CreateGroup();
        var groupId = group.Id != Guid.Empty ? group.Id : (Guid?)null;
        var result = ExpenseCategory.Create(
            name,
            description: null,
            icon: icon,
            color: color,
            groupId: groupId,
            userId: null,
            isEditable: true);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error.Description);
        }

        var category = result.Value;
        AttachGroup(category, group);

        if (id.HasValue)
        {
            SetEntityId(category, id.Value);
        }

        return category;
    }

    private static void AttachGroup(ExpenseCategory category, ExpenseCategoryGroup group)
    {
        var prop = typeof(ExpenseCategory).GetProperty(nameof(ExpenseCategory.Group))
                   ?? throw new InvalidOperationException("ExpenseCategory.Group not found");
        var setter = prop.GetSetMethod(nonPublic: true)
                     ?? throw new InvalidOperationException("ExpenseCategory.Group has no setter");
        setter.Invoke(category, new object?[] { group });
    }

    private static void SetEntityId(BaseAuditableEntityGuid entity, Guid id)
    {
        var prop = typeof(BaseAuditableEntityGuid).GetProperty(nameof(BaseAuditableEntityGuid.Id))
                   ?? throw new InvalidOperationException("Id not found");
        var setter = prop.GetSetMethod(nonPublic: true)
                     ?? throw new InvalidOperationException("Id has no setter");
        setter.Invoke(entity, new object[] { id });
    }

    public static Budget CreateBudget(
        string userId = DefaultUserId,
        Guid? categoryId = null,
        decimal amount = 1000m,
        bool isRecurrent = false,
        int month = 3,
        int year = 2024)
    {
        var result = Budget.Create(userId, categoryId ?? Guid.NewGuid(), amount, isRecurrent, month, year);
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
