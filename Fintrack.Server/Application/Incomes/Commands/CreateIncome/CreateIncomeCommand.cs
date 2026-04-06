using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Incomes;

namespace Fintrack.Server.Application.Incomes.Commands.CreateIncome;

public record CreateIncomeCommand(
    string UserId,
    string Source,
    decimal Amount,
    Guid CategoryId,
    DateTime Date,
    string? Notes,
    bool IsRecurring,
    RecurringFrequency? Frequency = null,
    DateTime? NextDate = null
) : ICommand<Guid>;

internal sealed class CreateIncomeCommandValidator : AbstractValidator<CreateIncomeCommand>
{
    public CreateIncomeCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Source)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Source is required and must be at most 200 characters");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required");

        RuleFor(x => x.Frequency)
            .NotNull()
            .When(x => x.IsRecurring)
            .WithMessage("Frequency is required when the income is recurring.");
    }
}

internal sealed class CreateIncomeCommandHandler : ICommandHandler<CreateIncomeCommand, Guid>
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IRecurringIncomeRepository _recurringIncomeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIncomeCommandHandler(
        IIncomeRepository incomeRepository,
        IRecurringIncomeRepository recurringIncomeRepository,
        IUnitOfWork unitOfWork)
    {
        _incomeRepository = incomeRepository;
        _recurringIncomeRepository = recurringIncomeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateIncomeCommand request,
        CancellationToken cancellationToken)
    {
        var incomeResult = Income.Create(
            request.UserId,
            request.Source,
            request.Amount,
            request.CategoryId,
            request.Date,
            request.Notes);

        if (incomeResult.IsFailure)
        {
            return Result.Failure<Guid>(incomeResult.Error);
        }

        var income = incomeResult.Value;
        _incomeRepository.Add(income);

        if (request.IsRecurring && request.Frequency.HasValue)
        {
            var nextDate = request.NextDate ?? CalculateInitialNextDate(request.Date, request.Frequency.Value);

            var recurringTemplate = new RecurringIncome
            {
                UserId = request.UserId,
                Source = request.Source,
                Amount = request.Amount,
                CategoryId = request.CategoryId,
                Frequency = request.Frequency.Value,
                StartDate = request.Date,
                NextProcessingDate = nextDate,
                IsActive = true
            };

            _recurringIncomeRepository.Add(recurringTemplate);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return income.Id;
    }

    private static DateTime CalculateInitialNextDate(DateTime startDate, RecurringFrequency frequency)
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
