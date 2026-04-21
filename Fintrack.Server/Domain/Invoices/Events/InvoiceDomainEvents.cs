using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Invoices.Events;

public sealed record InvoiceCreatedDomainEvent(Guid InvoiceId) : IDomainEvent;
