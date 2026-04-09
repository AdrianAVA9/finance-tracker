namespace Fintrack.Server.Application.Abstractions.Storage;

/// <summary>
/// Persists uploaded receipt bytes until a background worker runs OCR.
/// <see cref="StorageRelativePath"/> is an opaque path segment stored on <see cref="Domain.Expenses.PendingReceiptExtractionJob"/>.
/// </summary>
public interface IReceiptPendingFileStore
{
    /// <returns>Relative path token to persist on the job entity.</returns>
    Task<string> SaveAsync(Stream content, string originalExtension, CancellationToken cancellationToken = default);

    Stream OpenRead(string storageRelativePath);

    void TryDelete(string storageRelativePath);
}
