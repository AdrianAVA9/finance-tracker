using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.IncomeCategories;

public static class IncomeCategoryErrors
{
    public static readonly Error NotFound = new(
        "IncomeCategory.NotFound",
        "The income category was not found.");

    public static readonly Error AccessDenied = new(
        "IncomeCategory.AccessDenied",
        "You do not have permission to access this income category.");

    public static readonly Error NameRequired = new(
        "IncomeCategory.NameRequired",
        "Income category name is required");

    public static readonly Error NameTooLong = new(
        "IncomeCategory.NameTooLong",
        "Income category name cannot exceed 200 characters");

    public static readonly Error NotEditable = new(
        "IncomeCategory.NotEditable",
        "This income category cannot be modified");

    public static readonly Error NotDeletable = new(
        "IncomeCategory.NotDeletable",
        "This income category cannot be deleted");
}
