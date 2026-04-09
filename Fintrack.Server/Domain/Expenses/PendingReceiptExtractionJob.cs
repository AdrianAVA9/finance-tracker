using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;

namespace Fintrack.Server.Domain.Expenses;

public sealed class PendingReceiptExtractionJob : BaseAuditableEntityGuid
{
    public string UserId { get; private set; } = string.Empty;
    public string StorageRelativePath { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public DocumentExtractionJobStatus Status { get; private set; }
    public Guid? ResultExpenseId { get; private set; }
    public string? ErrorMessage { get; private set; }

    private PendingReceiptExtractionJob()
        : base()
    {
    }

    public static Result<PendingReceiptExtractionJob> Create(
        string userId,
        string storageRelativePath,
        string mimeType)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure<PendingReceiptExtractionJob>(ExpenseErrors.UserIdRequired);
        }

        if (string.IsNullOrWhiteSpace(storageRelativePath))
        {
            return Result.Failure<PendingReceiptExtractionJob>(ExpenseErrors.PendingJobInvalidPath);
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            return Result.Failure<PendingReceiptExtractionJob>(ExpenseErrors.PendingJobInvalidMimeType);
        }

        var job = new PendingReceiptExtractionJob
        {
            Id = Guid.NewGuid(),
            UserId = userId.Trim(),
            StorageRelativePath = storageRelativePath.Trim(),
            MimeType = mimeType.Trim(),
            Status = DocumentExtractionJobStatus.Queued
        };

        return job;
    }

    public void MarkProcessing()
    {
        Status = DocumentExtractionJobStatus.Processing;
    }

    public void MarkCompleted(Guid expenseId)
    {
        Status = DocumentExtractionJobStatus.Completed;
        ResultExpenseId = expenseId;
        ErrorMessage = null;
    }

    public void MarkFailed(string errorMessage)
    {
        Status = DocumentExtractionJobStatus.Failed;
        ErrorMessage = errorMessage;
        ResultExpenseId = null;
    }
}
