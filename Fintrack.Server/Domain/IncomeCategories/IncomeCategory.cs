using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories.Events;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.IncomeCategories;

public sealed class IncomeCategory : BaseAuditableEntityGuid
{
    private const int MaxNameLength = 200;

    public string? UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Icon { get; private set; }
    public string? Color { get; private set; }
    public bool IsEditable { get; private set; }

    public ApplicationUser? User { get; private set; }
    public ICollection<Income> Incomes { get; private set; } = new List<Income>();
    public ICollection<RecurringIncome> RecurringIncomes { get; private set; } = new List<RecurringIncome>();

    private IncomeCategory()
        : base()
    {
    }

    private IncomeCategory(
        string name,
        string? icon,
        string? color,
        string? userId,
        bool isEditable)
        : base()
    {
        Id = Guid.NewGuid();
        Name = name;
        Icon = icon;
        Color = color;
        UserId = userId;
        IsEditable = isEditable;
    }

    public static Result<IncomeCategory> Create(
        string name,
        string? icon,
        string? color,
        string? userId,
        bool isEditable)
    {
        var validation = ValidateName(name);
        if (validation.IsFailure)
        {
            return Result.Failure<IncomeCategory>(validation.Error);
        }

        var category = new IncomeCategory(
            name.Trim(),
            icon,
            color,
            userId,
            isEditable);

        category.RaiseDomainEvent(new IncomeCategoryCreatedDomainEvent(category.Id));

        return category;
    }

    public Result Update(string name, string? icon, string? color)
    {
        if (!IsEditable)
        {
            return Result.Failure(IncomeCategoryErrors.NotEditable);
        }

        var validation = ValidateName(name);
        if (validation.IsFailure)
        {
            return validation;
        }

        Name = name.Trim();
        Icon = icon;
        Color = color;

        RaiseDomainEvent(new IncomeCategoryUpdatedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// Registers deletion before <see cref="IIncomeCategoryRepository.Remove"/>.
    /// </summary>
    public Result RegisterDeletion()
    {
        if (!IsEditable)
        {
            return Result.Failure(IncomeCategoryErrors.NotDeletable);
        }

        RaiseDomainEvent(new IncomeCategoryDeletedDomainEvent(Id));

        return Result.Success();
    }

    private static Result ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(IncomeCategoryErrors.NameRequired);
        }

        if (name.Trim().Length > MaxNameLength)
        {
            return Result.Failure(IncomeCategoryErrors.NameTooLong);
        }

        return Result.Success();
    }
}
