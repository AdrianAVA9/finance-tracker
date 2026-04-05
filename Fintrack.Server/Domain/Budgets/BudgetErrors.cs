using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Budgets;

public static class BudgetErrors
{
    public static readonly Error NotFound = new(
        "Budget.NotFound",
        "The budget with the specified identifier was not found");

    public static readonly Error NegativeAmount = new(
        "Budget.NegativeAmount",
        "The budget amount cannot be negative");

    public static readonly Error InvalidMonth = new(
        "Budget.InvalidMonth",
        "The month must be between 1 and 12");

    public static readonly Error InvalidYear = new(
        "Budget.InvalidYear",
        "The year must be a valid four-digit year");

    public static readonly Error AlreadyExists = new(
        "Budget.AlreadyExists",
        "A budget for this category, month, and year already exists");
}
