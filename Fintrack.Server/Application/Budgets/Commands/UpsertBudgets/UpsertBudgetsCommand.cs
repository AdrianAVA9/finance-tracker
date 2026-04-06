using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Application.Budgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;

namespace Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;

public record BudgetEntryDto(int CategoryId, decimal Amount, bool IsRecurrent = false);

public record UpsertBudgetsCommand(
    string UserId,
    int Month,
    int Year,
    List<BudgetEntryDto> Budgets
) : ICommand;

internal sealed class UpsertBudgetsCommandHandler : ICommandHandler<UpsertBudgetsCommand>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertBudgetsCommandHandler(IBudgetRepository budgetRepository, IUnitOfWork unitOfWork)
    {
        _budgetRepository = budgetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpsertBudgetsCommand request, CancellationToken cancellationToken)
    {
        var userBudgets = await _budgetRepository.GetUserBudgetsByMonthAsync(
            request.UserId,
            request.Month,
            request.Year,
            cancellationToken);

        var existingBudgets = userBudgets.ToDictionary(b => b.CategoryId);

        var uniqueBudgets = request.Budgets
            .GroupBy(b => b.CategoryId)
            .Select(g => g.Last())
            .ToList();

        foreach (var entry in uniqueBudgets)
        {
            if (existingBudgets.TryGetValue(entry.CategoryId, out var existing))
            {
                var updateResult = existing.Update(entry.Amount, entry.IsRecurrent);
                if (updateResult.IsFailure)
                {
                    return updateResult;
                }

                _budgetRepository.Update(existing);
            }
            else
            {
                var addResult = await BudgetSlotHelper.TryAddNewBudgetSlotAsync(
                    _budgetRepository,
                    request.UserId,
                    entry.CategoryId,
                    entry.Amount,
                    entry.IsRecurrent,
                    request.Month,
                    request.Year,
                    cancellationToken);

                if (addResult.IsFailure)
                {
                    return addResult;
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
