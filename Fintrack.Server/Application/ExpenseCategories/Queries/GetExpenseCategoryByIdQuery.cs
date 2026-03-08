using Fintrack.Server.Models;
using MediatR;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Queries
{
    public record GetExpenseCategoryByIdQuery(int Id, string UserId) : IRequest<ExpenseCategory>;

    internal sealed class GetExpenseCategoryByIdQueryHandler : IRequestHandler<GetExpenseCategoryByIdQuery, ExpenseCategory>
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;

        public GetExpenseCategoryByIdQueryHandler(IExpenseCategoryRepository expenseCategoryRepository)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
        }

        public async Task<ExpenseCategory> Handle(GetExpenseCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _expenseCategoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (category is null)
            {
                throw new NotFoundException(nameof(ExpenseCategory), request.Id);
            }

             if (category.UserId != request.UserId && !category.IsSystem)
            {
                throw new UnauthorizedAccessException("You do not have permission to view this category.");
            }

            return category;
        }
    }
}
