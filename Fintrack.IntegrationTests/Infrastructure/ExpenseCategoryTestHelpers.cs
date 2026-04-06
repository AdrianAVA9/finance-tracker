using System.Reflection;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.IntegrationTests.Infrastructure;

/// <summary>
/// Builds <see cref="ExpenseCategory"/> for integration tests after the parent <see cref="ExpenseCategoryGroup"/> is persisted (so <see cref="ExpenseCategoryGroup.Id"/> is set).
/// </summary>
internal static class ExpenseCategoryTestHelpers
{
    public static ExpenseCategory CreateWithGroup(
        ExpenseCategoryGroup group,
        string name = "Test category",
        string? userId = null,
        bool isEditable = true)
    {
        var result = ExpenseCategory.Create(
            name,
            description: null,
            icon: null,
            color: null,
            groupId: group.Id,
            userId: userId,
            isEditable: isEditable);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error.Description);
        }

        var category = result.Value;
        AttachGroup(category, group);
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
}
