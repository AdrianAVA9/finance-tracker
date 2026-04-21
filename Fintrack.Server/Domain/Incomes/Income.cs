using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Domain.Incomes.Events;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Incomes;

public sealed class Income : BaseAuditableEntityGuid
{
    private const int MaxSourceLength = 200;

    public string UserId { get; private set; } = string.Empty;
    public string Source { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTime Date { get; private set; }
    public string? Notes { get; private set; }

    public ApplicationUser? User { get; private set; }
    public IncomeCategory? Category { get; private set; }

    private Income()
        : base()
    {
    }

    private Income(
        string userId,
        string source,
        decimal amount,
        Guid categoryId,
        DateTime date,
        string? notes)
        : base()
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Source = source;
        Amount = amount;
        CategoryId = categoryId;
        Date = date;
        Notes = notes;
    }

    public static Result<Income> Create(
        string userId,
        string source,
        decimal amount,
        Guid categoryId,
        DateTime date,
        string? notes)
    {
        var validation = ValidateCommon(userId, source, amount, categoryId, date);
        if (validation.IsFailure)
        {
            return Result.Failure<Income>(validation.Error);
        }

        var income = new Income(
            userId.Trim(),
            source.Trim(),
            amount,
            categoryId,
            date,
            notes);

        income.RaiseDomainEvent(new IncomeCreatedDomainEvent(income.Id));

        return income;
    }

    public Result Update(
        string source,
        decimal amount,
        Guid categoryId,
        DateTime date,
        string? notes)
    {
        var validation = ValidateCommon(UserId, source, amount, categoryId, date);
        if (validation.IsFailure)
        {
            return validation;
        }

        Source = source.Trim();
        Amount = amount;
        CategoryId = categoryId;
        Date = date;
        Notes = notes;

        RaiseDomainEvent(new IncomeUpdatedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// Registers that this income will be removed. Raises <see cref="IncomeDeletedDomainEvent"/>.
    /// Call immediately before <see cref="IIncomeRepository.Remove"/> so the event is dispatched with save.
    /// </summary>
    public void RegisterDeletion()
    {
        RaiseDomainEvent(new IncomeDeletedDomainEvent(Id));
    }

    private static Result ValidateCommon(
        string userId,
        string source,
        decimal amount,
        Guid categoryId,
        DateTime date)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(IncomeErrors.UserIdRequired);
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            return Result.Failure(IncomeErrors.SourceRequired);
        }

        if (source.Trim().Length > MaxSourceLength)
        {
            return Result.Failure(IncomeErrors.SourceTooLong);
        }

        if (amount <= 0)
        {
            return Result.Failure(IncomeErrors.InvalidAmount);
        }

        if (categoryId == Guid.Empty)
        {
            return Result.Failure(IncomeErrors.CategoryRequired);
        }

        var today = DateTime.UtcNow.Date;
        if (date.Date > today.AddYears(1))
        {
            return Result.Failure(IncomeErrors.DateTooFarInFuture);
        }

        return Result.Success();
    }
}
