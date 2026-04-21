using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Incomes;

namespace Fintrack.Server.Application.Incomes.Commands.DeleteIncome;

public record DeleteIncomeCommand(Guid Id, string UserId) : ICommand;

internal sealed class DeleteIncomeCommandValidator : AbstractValidator<DeleteIncomeCommand>
{
    public DeleteIncomeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Income ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

internal sealed class DeleteIncomeCommandHandler : ICommandHandler<DeleteIncomeCommand>
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIncomeCommandHandler(IIncomeRepository incomeRepository, IUnitOfWork unitOfWork)
    {
        _incomeRepository = incomeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteIncomeCommand request, CancellationToken cancellationToken)
    {
        var income = await _incomeRepository.GetByIdAsync(
            request.Id,
            request.UserId,
            cancellationToken);

        if (income is null)
        {
            return Result.Failure(IncomeErrors.NotFound);
        }

        income.RegisterDeletion();
        _incomeRepository.Remove(income);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
