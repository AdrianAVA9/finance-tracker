using MediatR;
using Fintrack.Server.Data;
using Fintrack.Server.Models;
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
