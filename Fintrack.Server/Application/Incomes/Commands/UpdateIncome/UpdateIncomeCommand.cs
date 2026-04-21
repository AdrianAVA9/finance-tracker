using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Incomes;

namespace Fintrack.Server.Application.Incomes.Commands.UpdateIncome;

public record UpdateIncomeCommand(
    Guid Id,
    string UserId,
    string Source,
    decimal Amount,
    Guid CategoryId,
    DateTime Date,
    string? Notes,
    bool IsRecurring,
    RecurringFrequency? Frequency,
    DateTime? NextDate
) : ICommand;

internal sealed class UpdateIncomeCommandValidator : AbstractValidator<UpdateIncomeCommand>
{
    public UpdateIncomeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Income ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Source)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Frequency)
            .NotNull()
            .When(x => x.IsRecurring)
            .WithMessage("La frecuencia es requerida cuando el ingreso es recurrente.");
    }
}

internal sealed class UpdateIncomeCommandHandler : ICommandHandler<UpdateIncomeCommand>
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IRecurringIncomeRepository _recurringIncomeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncomeCommandHandler(
        IIncomeRepository incomeRepository,
        IRecurringIncomeRepository recurringIncomeRepository,
        IUnitOfWork unitOfWork)
    {
        _incomeRepository = incomeRepository;
        _recurringIncomeRepository = recurringIncomeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateIncomeCommand request, CancellationToken cancellationToken)
    {
        var income = await _incomeRepository.GetByIdAsync(
            request.Id,
            request.UserId,
            cancellationToken);

        if (income is null)
        {
            return Result.Failure(IncomeErrors.NotFound);
        }

        var existingRecurring = await _recurringIncomeRepository.FindActiveMatchingTemplateAsync(
            request.UserId,
            income.Source,
            income.CategoryId,
            income.Amount,
            cancellationToken);

        var updateResult = income.Update(
            request.Source,
            request.Amount,
            request.CategoryId,
            request.Date,
            request.Notes);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        if (request.IsRecurring)
        {
            if (existingRecurring is not null)
            {
                existingRecurring.Source = request.Source;
                existingRecurring.Amount = request.Amount;
                existingRecurring.CategoryId = request.CategoryId;
                existingRecurring.Frequency = request.Frequency ?? RecurringFrequency.Monthly;
                if (request.NextDate.HasValue)
                {
                    existingRecurring.NextProcessingDate = request.NextDate.Value;
                }
            }
            else
            {
                var recurringTemplate = new RecurringIncome
                {
                    UserId = request.UserId,
                    Source = request.Source,
                    Amount = request.Amount,
                    CategoryId = request.CategoryId,
                    Frequency = request.Frequency ?? RecurringFrequency.Monthly,
                    StartDate = request.Date,
                    NextProcessingDate = request.NextDate ?? CalculateNextDate(
                        request.Date,
                        request.Frequency ?? RecurringFrequency.Monthly),
                    IsActive = true
                };
                _recurringIncomeRepository.Add(recurringTemplate);
            }
        }
        else if (existingRecurring is not null)
        {
            existingRecurring.IsActive = false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static DateTime CalculateNextDate(DateTime startDate, RecurringFrequency frequency)
    {
        return frequency switch
        {
            RecurringFrequency.Daily => startDate.AddDays(1),
            RecurringFrequency.Weekly => startDate.AddDays(7),
            RecurringFrequency.BiWeekly => startDate.AddDays(14),
            RecurringFrequency.Monthly => startDate.AddMonths(1),
            RecurringFrequency.Quarterly => startDate.AddMonths(3),
            RecurringFrequency.Yearly => startDate.AddYears(1),
            _ => startDate.AddMonths(1)
        };
    }
}
