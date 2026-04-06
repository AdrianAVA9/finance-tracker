using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets.Events;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Budgets;

public sealed class Budget : BaseAuditableEntityGuid
{
    public string UserId { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsRecurrent { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    public ApplicationUser? User { get; private set; }
    public ExpenseCategory? Category { get; private set; }

    private Budget()
        : base()
    {
    }

    private Budget(
        string userId,
        Guid categoryId,
        decimal amount,
        bool isRecurrent,
        int month,
        int year)
        : base()
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CategoryId = categoryId;
        Amount = amount;
        IsRecurrent = isRecurrent;
        Month = month;
        Year = year;
    }

    public static Result<Budget> Create(
        string userId,
        Guid categoryId,
        decimal amount,
        bool isRecurrent,
        int month,
        int year)
    {
        if (amount < 0)
        {
            return Result.Failure<Budget>(BudgetErrors.NegativeAmount);
        }

        if (month < 1 || month > 12)
        {
            return Result.Failure<Budget>(BudgetErrors.InvalidMonth);
        }

        if (year < 1900 || year > 2100)
        {
            return Result.Failure<Budget>(BudgetErrors.InvalidYear);
        }

        var budget = new Budget(
            userId,
            categoryId,
            amount,
            isRecurrent,
            month,
            year);

        budget.RaiseDomainEvent(new BudgetCreatedDomainEvent(budget.Id));

        return budget;
    }

    public Result Update(decimal amount, bool isRecurrent)
    {
        if (amount < 0)
        {
            return Result.Failure(BudgetErrors.NegativeAmount);
        }

        Amount = amount;
        IsRecurrent = isRecurrent;

        RaiseDomainEvent(new BudgetUpdatedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// Registers that this budget will be removed. Raises <see cref="BudgetDeletedDomainEvent"/>.
    /// Call immediately before <see cref="IBudgetRepository.Remove"/> so the event is dispatched with save.
    /// </summary>
    public void RegisterDeletion()
    {
        RaiseDomainEvent(new BudgetDeletedDomainEvent(Id));
    }
}
