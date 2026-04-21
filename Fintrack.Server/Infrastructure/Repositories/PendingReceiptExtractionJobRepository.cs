using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Repositories;

internal sealed class PendingReceiptExtractionJobRepository : IPendingReceiptExtractionJobRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PendingReceiptExtractionJobRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PendingReceiptExtractionJob?> GetOldestQueuedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PendingReceiptExtractionJobs
            .Where(j => j.Status == DocumentExtractionJobStatus.Queued)
            .OrderBy(j => j.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PendingReceiptExtractionJob?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PendingReceiptExtractionJobs
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId, cancellationToken);
    }

    public void Add(PendingReceiptExtractionJob job)
    {
        _dbContext.PendingReceiptExtractionJobs.Add(job);
    }
}
