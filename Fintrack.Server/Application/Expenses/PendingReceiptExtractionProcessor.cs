using Fintrack.Server.Application.Abstractions.Storage;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;
using Microsoft.Extensions.Logging;

namespace Fintrack.Server.Application.Expenses;

/// <summary>
/// Picks one queued receipt job, runs OCR/expense creation, updates job status.
/// Intended to be invoked periodically from a background host.
/// </summary>
public sealed class PendingReceiptExtractionProcessor
{
    private readonly IPendingReceiptExtractionJobRepository _jobs;
    private readonly IReceiptPendingFileStore _fileStore;
    private readonly ReceiptProcessingService _receiptProcessing;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PendingReceiptExtractionProcessor> _logger;

    public PendingReceiptExtractionProcessor(
        IPendingReceiptExtractionJobRepository jobs,
        IReceiptPendingFileStore fileStore,
        ReceiptProcessingService receiptProcessing,
        IUnitOfWork unitOfWork,
        ILogger<PendingReceiptExtractionProcessor> logger)
    {
        _jobs = jobs;
        _fileStore = fileStore;
        _receiptProcessing = receiptProcessing;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ProcessNextIfAnyAsync(CancellationToken cancellationToken = default)
    {
        var job = await _jobs.GetOldestQueuedAsync(cancellationToken);
        if (job is null)
        {
            return;
        }

        job.MarkProcessing();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await using var stream = _fileStore.OpenRead(job.StorageRelativePath);
            var expense = await _receiptProcessing.ProcessReceiptAsync(
                stream,
                job.MimeType,
                job.UserId,
                cancellationToken);

            job.MarkCompleted(expense.Id);
            _fileStore.TryDelete(job.StorageRelativePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pending receipt job {JobId} failed", job.Id);
            job.MarkFailed(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
