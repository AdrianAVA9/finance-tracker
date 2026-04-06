using System.Text.Json.Serialization;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories.Events;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.ExpenseCategories;

public sealed class ExpenseCategoryGroup : BaseAuditableEntityGuid
{
    private const int MaxNameLength = 200;

    [JsonInclude]
    public string? UserId { get; private set; }
    [JsonInclude]
    public string Name { get; private set; } = string.Empty;
    [JsonInclude]
    public string? Description { get; private set; }
    [JsonInclude]
    public bool IsEditable { get; private set; }

    public ApplicationUser? User { get; private set; }
    public ICollection<ExpenseCategory> Categories { get; private set; } = new List<ExpenseCategory>();

    /// <summary>Parameterless constructor for EF Core and JSON deserialization.</summary>
    public ExpenseCategoryGroup()
        : base()
    {
    }

    private ExpenseCategoryGroup(string name, string? description, string? userId, bool isEditable)
        : base()
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        UserId = userId;
        IsEditable = isEditable;
    }

    /// <summary>
    /// Creates a user-owned group (editable by that user).
    /// </summary>
    public static Result<ExpenseCategoryGroup> CreateForUser(string name, string? description, string userId)
    {
        return CreateCore(name, description, userId, isEditable: true);
    }

    /// <summary>
    /// Creates a system-wide group (shared, not editable by end users).
    /// </summary>
    public static Result<ExpenseCategoryGroup> CreateForSystem(string name, string? description)
    {
        return CreateCore(name, description, userId: null, isEditable: false);
    }

    private static Result<ExpenseCategoryGroup> CreateCore(
        string name,
        string? description,
        string? userId,
        bool isEditable)
    {
        var validation = ValidateName(name);
        if (validation.IsFailure)
        {
            return Result.Failure<ExpenseCategoryGroup>(validation.Error);
        }

        var group = new ExpenseCategoryGroup(name.Trim(), description, userId, isEditable);
        group.RaiseDomainEvent(new ExpenseCategoryGroupCreatedDomainEvent(group.Id));

        return group;
    }

    public Result Update(string name, string? description)
    {
        if (!IsEditable)
        {
            return Result.Failure(ExpenseCategoryGroupErrors.NotEditable);
        }

        var validation = ValidateName(name);
        if (validation.IsFailure)
        {
            return validation;
        }

        Name = name.Trim();
        Description = description;

        RaiseDomainEvent(new ExpenseCategoryGroupUpdatedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// Registers deletion before <see cref="IExpenseCategoryGroupRepository.Remove"/>.
    /// </summary>
    public Result RegisterDeletion()
    {
        if (!IsEditable)
        {
            return Result.Failure(ExpenseCategoryGroupErrors.NotEditable);
        }

        RaiseDomainEvent(new ExpenseCategoryGroupDeletedDomainEvent(Id));

        return Result.Success();
    }

    private static Result ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(ExpenseCategoryGroupErrors.NameRequired);
        }

        if (name.Trim().Length > MaxNameLength)
        {
            return Result.Failure(ExpenseCategoryGroupErrors.NameTooLong);
        }

        return Result.Success();
    }
}
