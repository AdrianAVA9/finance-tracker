using Fintrack.Server.Application.Abstractions.Clock;

namespace Fintrack.Server.Infrastructure.Clock;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
