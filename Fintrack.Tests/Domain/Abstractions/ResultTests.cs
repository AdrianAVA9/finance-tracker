using Fintrack.Server.Domain.Abstractions;
using FluentAssertions;

namespace Fintrack.Tests.Domain.Abstractions;

public sealed class ResultTests
{
    [Fact]
    public void Success_Should_BeSuccessful_And_UseNoneError()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_Should_BeFailed_And_CarryError()
    {
        var error = new Error("code", "desc");

        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_WithNone_Should_Throw()
    {
        var act = () => Result.Failure(Error.None);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Generic_Success_Should_ExposeValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Generic_Failure_AccessingValue_Should_Throw()
    {
        var result = Result.Failure<int>(new Error("x", "y"));

        var act = () => _ = result.Value;

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot access value of a failed result*");
    }

    [Fact]
    public void Generic_Failure_Should_InheritFailureState()
    {
        var error = new Error("e", "m");
        var result = Result.Failure<string>(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Implicit_FromNonNull_Should_Succeed()
    {
        Result<string> result = "ok";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public void Implicit_FromNull_Should_FailWithNullValueError()
    {
        Result<string?> result = (string?)null;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }
}
