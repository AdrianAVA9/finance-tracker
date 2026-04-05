using MediatR;
using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
