using Fintrack.Server.Models;
using MediatR;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Queries
{
    public record GetExpenseCategoryGroupByIdQuery(int Id, string UserId) : IRequest<ExpenseCategoryGroup>;

    internal sealed class GetExpenseCategoryGroupByIdQueryHandler : IRequestHandler<GetExpenseCategoryGroupByIdQuery, ExpenseCategoryGroup>
    {
        private readonly IExpenseCategoryGroupRepository _repository;

        public GetExpenseCategoryGroupByIdQueryHandler(IExpenseCategoryGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExpenseCategoryGroup> Handle(GetExpenseCategoryGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var group = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (group == null)
                throw new NotFoundException(nameof(ExpenseCategoryGroup), request.Id);

            if (group.UserId != null && group.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to view this group.");

            return group;
        }
    }
}
