using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Budgets.Commands;

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

        // Group to handle safety if multiple items for same CategoryId are sent in one batch
        var uniqueBudgets = request.Budgets
            .GroupBy(b => b.CategoryId)
            .Select(g => g.Last()) // Take the last one if duplicates exist
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
                var createResult = Budget.Create(
                    request.UserId,
                    entry.CategoryId,
                    entry.Amount,
                    entry.IsRecurrent,
                    request.Month,
                    request.Year);

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
