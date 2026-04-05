using MediatR;
using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result> { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
