using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;

namespace Fintrack.Server.Application.IncomeCategories.Commands.UpdateIncomeCategory;

public record UpdateIncomeCategoryCommand(
    Guid Id,
    string Name,
    string? Icon,
    string? Color,
    string UserId
) : ICommand;

internal sealed class UpdateIncomeCategoryCommandHandler : ICommandHandler<UpdateIncomeCategoryCommand>
{
    private readonly IIncomeCategoryRepository _incomeCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncomeCategoryCommandHandler(
        IIncomeCategoryRepository incomeCategoryRepository,
        IUnitOfWork unitOfWork)
    {
        _incomeCategoryRepository = incomeCategoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateIncomeCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _incomeCategoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(IncomeCategoryErrors.NotFound);
        }

        if (category.UserId != null && category.UserId != request.UserId)
        {
            return Result.Failure(IncomeCategoryErrors.AccessDenied);
        }

        var updateResult = category.Update(request.Name, request.Icon, request.Color);

        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _incomeCategoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
