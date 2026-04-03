using MediatR;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Incomes.Queries
{
    public record IncomeCategoryDto(int Id, string Name, string? Icon, string? Color);

    public record GetIncomeCategoriesQuery(string UserId) : IRequest<List<IncomeCategoryDto>>;

    public class GetIncomeCategoriesQueryHandler : IRequestHandler<GetIncomeCategoriesQuery, List<IncomeCategoryDto>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetIncomeCategoriesQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<IncomeCategoryDto>> Handle(GetIncomeCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.IncomeCategories
                .Where(c => c.UserId == null || c.UserId == request.UserId)
                .Select(c => new IncomeCategoryDto(c.Id, c.Name, c.Icon, c.Color))
                .ToListAsync(cancellationToken);
        }
    }
}
