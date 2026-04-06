using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.ExpenseCategories;

public static class ExpenseCategoryGroupErrors
{
    public static readonly Error NotFound = new(
        "ExpenseCategoryGroup.NotFound",
        "The expense category group was not found.");

    public static readonly Error AccessDenied = new(
        "ExpenseCategoryGroup.AccessDenied",
        "You do not have permission to access this expense category group.");

    public static readonly Error NotEditable = new(
        "ExpenseCategoryGroup.NotEditable",
        "This expense category group cannot be modified.");
}
