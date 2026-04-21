using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Tests.TestData.Expenses;

internal static class ExpenseTestDoubles
{
    public const string DefaultUserId = "user-1";

    public static Expense CreateExpense(
        string userId = DefaultUserId,
        decimal totalAmount = 100m,
        DateTime? date = null,
        string? merchant = "Test Merchant",
        ExpenseStatus status = ExpenseStatus.Completed)
    {
        var result = Expense.Create(
            userId,
            totalAmount,
            date ?? DateTime.UtcNow,
            merchant,
            status: status);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error.Description);
        }

        return result.Value;
    }

    public static Expense CreateExpenseWithItems(
        string userId = DefaultUserId,
        decimal totalAmount = 100m,
        int itemCount = 2)
    {
        var expense = CreateExpense(userId, totalAmount);
        var amountPerItem = totalAmount / itemCount;

        for (int i = 0; i < itemCount; i++)
        {
            expense.AddItem(ExpenseItem.Create(
                Guid.NewGuid(),
                amountPerItem,
                $"Item {i + 1}"));
        }

        return expense;
    }

    /// <summary>
    /// Creates an <see cref="ExpenseItem"/> with its <see cref="ExpenseItem.Expense"/>
    /// navigation set—for test assertions that traverse the navigation (e.g. Budget details).
    /// </summary>
    public static ExpenseItem CreateItemWithExpense(
        Guid categoryId,
        decimal amount,
        string? description,
        Expense parentExpense)
    {
        var item = ExpenseItem.Create(categoryId, amount, description);
        SetNavigation(item, nameof(ExpenseItem.Expense), parentExpense);
        return item;
    }

    private static void SetEntityId(BaseAuditableEntityGuid entity, Guid id)
    {
        var prop = typeof(BaseAuditableEntityGuid).GetProperty(nameof(BaseAuditableEntityGuid.Id))
                   ?? throw new InvalidOperationException("Id not found");
        var setter = prop.GetSetMethod(nonPublic: true)
                     ?? throw new InvalidOperationException("Id has no setter");
        setter.Invoke(entity, new object[] { id });
    }

    private static void SetNavigation<TEntity, TNav>(TEntity entity, string propertyName, TNav value)
        where TEntity : class
    {
        var prop = typeof(TEntity).GetProperty(propertyName)
                   ?? throw new InvalidOperationException($"{typeof(TEntity).Name}.{propertyName} not found");
        var setter = prop.GetSetMethod(nonPublic: true)
                     ?? throw new InvalidOperationException($"{typeof(TEntity).Name}.{propertyName} has no setter");
        setter.Invoke(entity, new object?[] { value });
    }
}
