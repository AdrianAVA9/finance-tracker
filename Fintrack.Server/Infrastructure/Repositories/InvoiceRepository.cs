using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Repositories;

internal sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InvoiceRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Invoice?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invoices
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId, cancellationToken);
    }

    public async Task<Invoice?> GetByIdWithItemsAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId, cancellationToken);
    }

    public void Add(Invoice invoice)
    {
        _dbContext.Invoices.Add(invoice);
    }

    public void Update(Invoice invoice)
    {
        _dbContext.Invoices.Update(invoice);
    }

    public void Remove(Invoice invoice)
    {
        _dbContext.Invoices.Remove(invoice);
    }
}
