using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Application.Abstractions.Storage;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Server.Application.Expenses.Commands.EnqueuePendingReceipt;

public record EnqueuePendingReceiptCommand(
    string UserId,
    byte[] FileContent,
    string MimeType,
    string? OriginalFileName
) : ICommand<Guid>;

internal sealed class EnqueuePendingReceiptCommandValidator : AbstractValidator<EnqueuePendingReceiptCommand>
{
    public EnqueuePendingReceiptCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("File content is required");

        RuleFor(x => x.MimeType)
            .NotEmpty()
            .WithMessage("MIME type is required");
    }
}

internal sealed class EnqueuePendingReceiptCommandHandler : ICommandHandler<EnqueuePendingReceiptCommand, Guid>
{
    private readonly IReceiptPendingFileStore _fileStore;
    private readonly IPendingReceiptExtractionJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnqueuePendingReceiptCommandHandler(
        IReceiptPendingFileStore fileStore,
        IPendingReceiptExtractionJobRepository jobRepository,
        IUnitOfWork unitOfWork)
    {
        _fileStore = fileStore;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        EnqueuePendingReceiptCommand request,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.OriginalFileName ?? string.Empty);
        if (string.IsNullOrEmpty(extension))
        {
            extension = GuessExtensionFromMimeType(request.MimeType);
        }

        await using var uploadStream = new MemoryStream(request.FileContent, writable: false);
        var relativePath = await _fileStore.SaveAsync(uploadStream, extension, cancellationToken);

        var jobResult = PendingReceiptExtractionJob.Create(
            request.UserId,
            relativePath,
            request.MimeType);

        if (jobResult.IsFailure)
        {
            return Result.Failure<Guid>(jobResult.Error);
        }

        _jobRepository.Add(jobResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return jobResult.Value.Id;
    }

    private static string GuessExtensionFromMimeType(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            "application/pdf" => ".pdf",
            _ => ".bin"
        };
    }
}
