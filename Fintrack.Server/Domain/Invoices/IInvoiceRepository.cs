namespace Fintrack.Server.Domain.Invoices;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Invoice?> GetByIdWithItemsAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    void Add(Invoice invoice);

    void Update(Invoice invoice);

    void Remove(Invoice invoice);
}
