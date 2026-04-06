using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;

namespace Fintrack.Server.Application.Budgets;

/// <summary>
/// Shared path for inserting a budget when (user, category, month, year) has no row yet.
/// </summary>
internal static class BudgetSlotHelper
{
    public static async Task<Result> TryAddNewBudgetSlotAsync(
        IBudgetRepository repository,
        string userId,
        int categoryId,
        decimal amount,
        bool isRecurrent,
        int month,
        int year,
        CancellationToken cancellationToken)
    {
        if (await repository.ExistsAsync(userId, categoryId, month, year, cancellationToken))
        {
            return Result.Failure(BudgetErrors.AlreadyExists);
        }

        var createResult = Budget.Create(
            userId,
            categoryId,
            amount,
            isRecurrent,
            month,
            year);

        if (createResult.IsFailure)
        {
            return createResult;
        }

        repository.Add(createResult.Value);
        return Result.Success();
    }
}
