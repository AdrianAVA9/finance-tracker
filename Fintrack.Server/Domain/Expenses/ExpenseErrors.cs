using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Expenses;

public static class ExpenseErrors
{
    public static readonly Error NotFound = new(
        "Expense.NotFound",
        "The expense with the specified identifier was not found");

    public static readonly Error UserIdRequired = new(
        "Expense.UserIdRequired",
        "User ID is required to create an expense");

    public static readonly Error InvalidTotalAmount = new(
        "Expense.InvalidTotalAmount",
        "The expense total amount must be greater than zero");

    public static readonly Error ItemsSumMismatch = new(
        "Expense.ItemsSumMismatch",
        "The sum of item amounts does not equal the total amount");

    public static readonly Error ItemsRequired = new(
        "Expense.ItemsRequired",
        "An expense must contain at least one item");

    public static readonly Error InvoicePayloadInvalid = new(
        "Expense.InvoicePayloadInvalid",
        "The invoice payload is invalid or does not match the expense total");

    public static readonly Error PendingJobInvalidPath = new(
        "Expense.PendingJobInvalidPath",
        "Stored file path is required");

    public static readonly Error PendingJobInvalidMimeType = new(
        "Expense.PendingJobInvalidMimeType",
        "MIME type is required");

    public static readonly Error PendingJobNotFound = new(
        "Expense.PendingJobNotFound",
        "The receipt processing job was not found");
}
