using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Incomes;

public static class IncomeErrors
{
    public static readonly Error NotFound = new(
        "Income.NotFound",
        "The income with the specified identifier was not found");

    public static readonly Error UserIdRequired = new(
        "Income.UserIdRequired",
        "User ID is required");

    public static readonly Error SourceRequired = new(
        "Income.SourceRequired",
        "Income source is required");

    public static readonly Error SourceTooLong = new(
        "Income.SourceTooLong",
        "Income source cannot exceed 200 characters");

    public static readonly Error InvalidAmount = new(
        "Income.InvalidAmount",
        "The income amount must be greater than zero");

    public static readonly Error CategoryRequired = new(
        "Income.CategoryRequired",
        "A category is required");

    public static readonly Error DateTooFarInFuture = new(
        "Income.DateTooFarInFuture",
        "The income date cannot be more than one year in the future");
}
