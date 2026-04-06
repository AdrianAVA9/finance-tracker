using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Incomes;

namespace Fintrack.Server.Application.Incomes.Queries.GetIncomeById;

public record IncomeDetailsDto(
    Guid Id,
    string Source,
    decimal Amount,
    Guid CategoryId,
    DateTime Date,
    string? Notes,
    bool IsRecurring,
    RecurringFrequency? Frequency,
    DateTime? NextDate);

public record GetIncomeByIdQuery(Guid Id, string UserId) : IQuery<IncomeDetailsDto>;

internal sealed class GetIncomeByIdQueryValidator : AbstractValidator<GetIncomeByIdQuery>
{
    public GetIncomeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Income ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

internal sealed class GetIncomeByIdQueryHandler : IQueryHandler<GetIncomeByIdQuery, IncomeDetailsDto>
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IRecurringIncomeRepository _recurringIncomeRepository;

    public GetIncomeByIdQueryHandler(
        IIncomeRepository incomeRepository,
        IRecurringIncomeRepository recurringIncomeRepository)
    {
        _incomeRepository = incomeRepository;
        _recurringIncomeRepository = recurringIncomeRepository;
    }

    public async Task<Result<IncomeDetailsDto>> Handle(
        GetIncomeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var income = await _incomeRepository.GetByIdAsNoTrackingAsync(
            request.Id,
            request.UserId,
            cancellationToken);

        if (income is null)
        {
            return Result.Failure<IncomeDetailsDto>(IncomeErrors.NotFound);
        }

        var recurring = await _recurringIncomeRepository.FindActiveMatchingTemplateAsync(
            request.UserId,
            income.Source,
            income.CategoryId,
            income.Amount,
            cancellationToken);

        return new IncomeDetailsDto(
            income.Id,
            income.Source,
            income.Amount,
            income.CategoryId,
            income.Date,
            income.Notes,
            recurring is not null,
            recurring?.Frequency,
            recurring?.NextProcessingDate);
    }
}
