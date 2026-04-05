using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets.Events;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Budgets;

public sealed class Budget : BudgetAuditableEntity
{
    // Properties with private setters
    public string UserId { get; private set; } = string.Empty;
    public int CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public bool IsRecurrent { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    // Navigation properties
    public ApplicationUser? User { get; private set; }
    public ExpenseCategory? Category { get; private set; }

    // Private constructor for EF Core
    private Budget()
        : base()
    {
    }

    // Private constructor for factory method
    private Budget(
        string userId,
        int categoryId,
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

    /// <summary>
    /// Creates a new Budget with validation
    /// </summary>
    public static Result<Budget> Create(
        string userId,
        int categoryId,
        decimal amount,
        bool isRecurrent,
        int month,
        int year)
    {
        // Validation
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

    /// <summary>
    /// Updates the budget amount and recurrence
    /// </summary>
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
}
