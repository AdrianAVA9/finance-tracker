using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.ExpenseCategories;

public static class ExpenseCategoryErrors
{
    public static readonly Error NotFound = new(
        "ExpenseCategory.NotFound",
        "The expense category was not found.");

    public static readonly Error AccessDenied = new(
        "ExpenseCategory.AccessDenied",
        "You do not have permission to access this expense category.");

    public static readonly Error NameRequired = new(
        "ExpenseCategory.NameRequired",
        "Expense category name is required");

    public static readonly Error NameTooLong = new(
        "ExpenseCategory.NameTooLong",
        "Expense category name cannot exceed 200 characters");

    public static readonly Error NotEditable = new(
        "ExpenseCategory.NotEditable",
        "This expense category cannot be modified");

    public static readonly Error NotDeletable = new(
        "ExpenseCategory.NotDeletable",
        "This expense category cannot be deleted");
}
