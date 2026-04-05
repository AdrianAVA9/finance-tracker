using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;

namespace Fintrack.Server.Application.Budgets.Commands.DeleteBudget;

public record DeleteBudgetCommand(int Id, string UserId) : ICommand;

internal sealed class DeleteBudgetCommandHandler : ICommandHandler<DeleteBudgetCommand>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBudgetCommandHandler(IBudgetRepository budgetRepository, IUnitOfWork unitOfWork)
    {
        _budgetRepository = budgetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);

        if (budget != null)
        {
            _budgetRepository.Remove(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
