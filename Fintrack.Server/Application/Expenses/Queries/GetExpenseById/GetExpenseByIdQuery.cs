using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Server.Application.Expenses.Queries.GetExpenseById;

public record ExpenseItemDetailsDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    decimal ItemAmount,
    string? Description);

public record ExpenseDetailsDto(
    Guid Id,
    string? Merchant,
    decimal TotalAmount,
    DateTime Date,
    string StatusName,
    List<ExpenseItemDetailsDto> Items);

public record GetExpenseByIdQuery(Guid Id, string UserId) : IQuery<ExpenseDetailsDto>;

internal sealed class GetExpenseByIdQueryValidator : AbstractValidator<GetExpenseByIdQuery>
{
    public GetExpenseByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

internal sealed class GetExpenseByIdQueryHandler : IQueryHandler<GetExpenseByIdQuery, ExpenseDetailsDto>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpenseByIdQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<Result<ExpenseDetailsDto>> Handle(
        GetExpenseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var expense = await _expenseRepository.GetByIdWithItemsAsync(
            request.Id,
            request.UserId,
            cancellationToken);

        if (expense is null)
        {
            return Result.Failure<ExpenseDetailsDto>(ExpenseErrors.NotFound);
        }

        var dto = new ExpenseDetailsDto(
            expense.Id,
            expense.Merchant,
            expense.TotalAmount,
            expense.Date,
            expense.Status.ToString(),
            expense.Items.Select(i => new ExpenseItemDetailsDto(
                i.Id,
                i.CategoryId,
                i.Category?.Name ?? "Unknown",
                i.ItemAmount,
                i.Description)).ToList());

        return dto;
    }
}
