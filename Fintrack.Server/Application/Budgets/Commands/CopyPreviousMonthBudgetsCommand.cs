using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Budgets.Commands;

public record CopyPreviousMonthBudgetsCommand(
    string UserId,
    int TargetMonth,
    int TargetYear
) : ICommand;

internal sealed class CopyPreviousMonthBudgetsCommandHandler : ICommandHandler<CopyPreviousMonthBudgetsCommand>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CopyPreviousMonthBudgetsCommandHandler(IBudgetRepository budgetRepository, IUnitOfWork unitOfWork)
    {
        _budgetRepository = budgetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CopyPreviousMonthBudgetsCommand request, CancellationToken cancellationToken)
    {
        // Calculate previous month and year
        int prevMonth = request.TargetMonth - 1;
        int prevYear = request.TargetYear;

        if (prevMonth == 0)
        {
            prevMonth = 12;
            prevYear--;
        }

        // 1. Fetch source budgets
        var sourceBudgets = await _budgetRepository.GetUserBudgetsByMonthAsync(
            request.UserId, 
            prevMonth, 
            prevYear, 
            cancellationToken);

        if (!sourceBudgets.Any())
        {
            return Result.Success(); // Nothing to copy
        }

        // 2. Clear current target month if exists (optional strategy, here we Upsert/Merge)
        var existingTargetBudgetsList = await _budgetRepository.GetUserBudgetsByMonthAsync(
            request.UserId, 
            request.TargetMonth, 
            request.TargetYear, 
            cancellationToken);
            
        var existingTargetBudgets = existingTargetBudgetsList.ToDictionary(b => b.CategoryId);

        foreach (var source in sourceBudgets)
        {
            if (existingTargetBudgets.TryGetValue(source.CategoryId, out var existing))
            {
                var updateResult = existing.Update(source.Amount, source.IsRecurrent);
                if (updateResult.IsFailure)
                {
                    return updateResult;
                }
                
                _budgetRepository.Update(existing);
            }
            else
            {
                var createResult = Budget.Create(
                    request.UserId,
                    source.CategoryId,
                    source.Amount,
                    source.IsRecurrent,
                    request.TargetMonth,
                    request.TargetYear);

                if (createResult.IsFailure)
                {
                    return createResult;
                }

                _budgetRepository.Add(createResult.Value);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
