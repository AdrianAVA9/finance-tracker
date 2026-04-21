using NSubstitute;

namespace Fintrack.Tests.Abstractions;

/// <summary>
/// Shared setup for fast, isolated unit tests (no database or HTTP).
/// </summary>
public abstract class BaseUnitTest
{
    protected static CancellationToken CancellationToken => CancellationToken.None;

    protected static T Mock<T>()
        where T : class
        => Substitute.For<T>();
}
