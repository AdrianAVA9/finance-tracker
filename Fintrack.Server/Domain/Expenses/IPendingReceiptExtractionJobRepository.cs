namespace Fintrack.Server.Domain.Expenses;

public interface IPendingReceiptExtractionJobRepository
{
    Task<PendingReceiptExtractionJob?> GetOldestQueuedAsync(CancellationToken cancellationToken = default);

    Task<PendingReceiptExtractionJob?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    void Add(PendingReceiptExtractionJob job);
}
