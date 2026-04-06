using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;

namespace Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;

public record IncomeCategoryDto(Guid Id, string Name, string? Icon, string? Color, bool IsEditable);

public record GetIncomeCategoriesQuery(string UserId) : IQuery<IReadOnlyList<IncomeCategoryDto>>;

internal sealed class GetIncomeCategoriesQueryHandler
    : IQueryHandler<GetIncomeCategoriesQuery, IReadOnlyList<IncomeCategoryDto>>
{
    private readonly IIncomeCategoryRepository _incomeCategoryRepository;

    public GetIncomeCategoriesQueryHandler(IIncomeCategoryRepository incomeCategoryRepository)
    {
        _incomeCategoryRepository = incomeCategoryRepository;
    }

    public async Task<Result<IReadOnlyList<IncomeCategoryDto>>> Handle(
        GetIncomeCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _incomeCategoryRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
        var dtos = list
            .Select(c => new IncomeCategoryDto(c.Id, c.Name, c.Icon, c.Color, c.IsEditable))
            .ToList();

        return Result.Success<IReadOnlyList<IncomeCategoryDto>>(dtos);
    }
}
