using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;

namespace Fintrack.Server.Application.IncomeCategories.Queries.GetUserOwnedIncomeCategories;

public record GetUserOwnedIncomeCategoriesQuery(string UserId) : IQuery<IReadOnlyList<IncomeCategoryDto>>;

internal sealed class GetUserOwnedIncomeCategoriesQueryHandler
    : IQueryHandler<GetUserOwnedIncomeCategoriesQuery, IReadOnlyList<IncomeCategoryDto>>
{
    private readonly IIncomeCategoryRepository _incomeCategoryRepository;

    public GetUserOwnedIncomeCategoriesQueryHandler(IIncomeCategoryRepository incomeCategoryRepository)
    {
        _incomeCategoryRepository = incomeCategoryRepository;
    }

    public async Task<Result<IReadOnlyList<IncomeCategoryDto>>> Handle(
        GetUserOwnedIncomeCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _incomeCategoryRepository.GetUserOwnedByUserIdAsync(request.UserId, cancellationToken);
        var dtos = list
            .Select(c => new IncomeCategoryDto(c.Id, c.Name, c.Icon, c.Color, c.IsEditable))
            .ToList();

        return Result.Success<IReadOnlyList<IncomeCategoryDto>>(dtos);
    }
}
