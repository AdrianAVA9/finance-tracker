using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;

namespace Fintrack.Server.Application.IncomeCategories.Commands.CreateIncomeCategory;

public record CreateIncomeCategoryCommand(
    string Name,
    string? Icon,
    string? Color,
    string UserId
) : ICommand<Guid>;

internal sealed class CreateIncomeCategoryCommandHandler
    : ICommandHandler<CreateIncomeCategoryCommand, Guid>
{
    private readonly IIncomeCategoryRepository _incomeCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIncomeCategoryCommandHandler(
        IIncomeCategoryRepository incomeCategoryRepository,
        IUnitOfWork unitOfWork)
    {
        _incomeCategoryRepository = incomeCategoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateIncomeCategoryCommand request, CancellationToken cancellationToken)
    {
        var createResult = IncomeCategory.Create(
            request.Name,
            request.Icon,
            request.Color,
            request.UserId,
            isEditable: true);

        if (createResult.IsFailure)
        {
            return Result.Failure<Guid>(createResult.Error);
        }

        var category = createResult.Value;

        _incomeCategoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
