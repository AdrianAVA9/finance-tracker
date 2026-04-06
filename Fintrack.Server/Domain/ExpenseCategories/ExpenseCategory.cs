using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories.Events;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.ExpenseCategories;

public sealed class ExpenseCategory : BaseAuditableEntityGuid
{
    private const int MaxNameLength = 200;

    public string? UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Color { get; private set; }
    public string? Icon { get; private set; }
    public string? Description { get; private set; }
    public int? GroupId { get; private set; }
    public bool IsEditable { get; private set; }

    public ApplicationUser? User { get; private set; }
    public ExpenseCategoryGroup? Group { get; private set; }
    public ICollection<Budget> Budgets { get; private set; } = new List<Budget>();

    private ExpenseCategory()
        : base()
    {
    }

    private ExpenseCategory(
        string name,
        string? description,
        string? icon,
        string? color,
        int? groupId,
        string? userId,
        bool isEditable)
        : base()
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Icon = icon;
        Color = color;
        GroupId = groupId;
        UserId = userId;
        IsEditable = isEditable;
    }

    /// <summary>
    /// Creates a user-defined or system-seeded expense category.
    /// </summary>
    public static Result<ExpenseCategory> Create(
        string name,
        string? description,
        string? icon,
        string? color,
        int? groupId,
        string? userId,
        bool isEditable)
    {
        var validation = ValidateName(name);
        if (validation.IsFailure)
        {
            return Result.Failure<ExpenseCategory>(validation.Error);
        }

        var category = new ExpenseCategory(
            name.Trim(),
            description,
            icon,
            color,
            groupId,
            userId,
            isEditable);

        category.RaiseDomainEvent(new ExpenseCategoryCreatedDomainEvent(category.Id));

        return category;
    }

    public Result Update(string name, string? description, string? icon, string? color, int? groupId)
    {
        if (!IsEditable)
        {
            return Result.Failure(ExpenseCategoryErrors.NotEditable);
        }

        var validation = ValidateName(name);
        if (validation.IsFailure)
        {
            return validation;
        }

        Name = name.Trim();
        Description = description;
        Icon = icon;
        Color = color;
        GroupId = groupId;

        RaiseDomainEvent(new ExpenseCategoryUpdatedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// Registers deletion before <see cref="IExpenseCategoryRepository.Remove"/>.
    /// </summary>
    public Result RegisterDeletion()
    {
        if (!IsEditable)
        {
            return Result.Failure(ExpenseCategoryErrors.NotDeletable);
        }

        RaiseDomainEvent(new ExpenseCategoryDeletedDomainEvent(Id));

        return Result.Success();
    }

    private static Result ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(ExpenseCategoryErrors.NameRequired);
        }

        if (name.Trim().Length > MaxNameLength)
        {
            return Result.Failure(ExpenseCategoryErrors.NameTooLong);
        }

        return Result.Success();
    }
}
