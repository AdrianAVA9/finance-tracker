using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;

namespace Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategoryById;

public record GetIncomeCategoryByIdQuery(Guid Id, string UserId) : IQuery<IncomeCategoryDto>;

internal sealed class GetIncomeCategoryByIdQueryHandler
    : IQueryHandler<GetIncomeCategoryByIdQuery, IncomeCategoryDto>
{
    private readonly IIncomeCategoryRepository _incomeCategoryRepository;

    public GetIncomeCategoryByIdQueryHandler(IIncomeCategoryRepository incomeCategoryRepository)
    {
        _incomeCategoryRepository = incomeCategoryRepository;
    }

    public async Task<Result<IncomeCategoryDto>> Handle(
        GetIncomeCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _incomeCategoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure<IncomeCategoryDto>(IncomeCategoryErrors.NotFound);
        }

        if (category.UserId != null && category.UserId != request.UserId)
        {
            return Result.Failure<IncomeCategoryDto>(IncomeCategoryErrors.AccessDenied);
        }

        var dto = new IncomeCategoryDto(
            category.Id,
            category.Name,
            category.Icon,
            category.Color,
            category.IsEditable);

        return Result.Success(dto);
    }
}
