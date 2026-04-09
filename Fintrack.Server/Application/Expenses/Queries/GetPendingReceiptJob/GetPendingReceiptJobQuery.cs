using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Server.Application.Expenses.Queries.GetPendingReceiptJob;

public record PendingReceiptJobDto(
    Guid Id,
    string Status,
    Guid? ResultExpenseId,
    string? ErrorMessage,
    DateTimeOffset CreatedAt);

public record GetPendingReceiptJobQuery(Guid JobId, string UserId) : IQuery<PendingReceiptJobDto>;

internal sealed class GetPendingReceiptJobQueryValidator : AbstractValidator<GetPendingReceiptJobQuery>
{
    public GetPendingReceiptJobQueryValidator()
    {
        RuleFor(x => x.JobId)
            .NotEqual(Guid.Empty)
            .WithMessage("Job ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

internal sealed class GetPendingReceiptJobQueryHandler : IQueryHandler<GetPendingReceiptJobQuery, PendingReceiptJobDto>
{
    private readonly IPendingReceiptExtractionJobRepository _jobRepository;

    public GetPendingReceiptJobQueryHandler(IPendingReceiptExtractionJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<Result<PendingReceiptJobDto>> Handle(
        GetPendingReceiptJobQuery request,
        CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.JobId, request.UserId, cancellationToken);
        if (job is null)
        {
            return Result.Failure<PendingReceiptJobDto>(ExpenseErrors.PendingJobNotFound);
        }

        return new PendingReceiptJobDto(
            job.Id,
            job.Status.ToString(),
            job.ResultExpenseId,
            job.ErrorMessage,
            job.CreatedAt);
    }
}
