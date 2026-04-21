using MediatR;
using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse> { }
