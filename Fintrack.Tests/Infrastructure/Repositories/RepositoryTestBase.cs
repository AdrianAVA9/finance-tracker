using Fintrack.Server.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Fintrack.Tests.Infrastructure.Repositories;

/// <summary>
/// Shared base for repository tests. Provides an EF Core InMemory
/// <see cref="ApplicationDbContext"/> with a mocked <see cref="IPublisher"/>
/// so domain events do not interfere with data-access assertions.
/// <para>
/// Unique constraints and provider-specific SQL are <b>not</b> validated
/// by InMemory — cover those in integration tests with PostgreSQL.
/// </para>
/// </summary>
public abstract class RepositoryTestBase
{
    protected static ApplicationDbContext CreateContext()
    {
        var publisher = Substitute.For<IPublisher>();
        publisher.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, publisher);
    }
}
