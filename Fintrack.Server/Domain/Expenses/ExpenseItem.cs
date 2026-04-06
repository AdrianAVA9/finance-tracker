using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Domain.Expenses;

public sealed class ExpenseItem : BaseAuditableEntityGuid
{
    public Guid ExpenseId { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal ItemAmount { get; private set; }
    public string? Description { get; private set; }

    public Expense? Expense { get; private set; }
    public ExpenseCategory? Category { get; private set; }

    private ExpenseItem() : base()
    {
    }

    private ExpenseItem(
        Guid categoryId,
        decimal itemAmount,
        string? description)
        : base()
    {
        Id = Guid.NewGuid();
        CategoryId = categoryId;
        ItemAmount = itemAmount;
        Description = description;
    }

    public static ExpenseItem Create(
        Guid categoryId,
        decimal itemAmount,
        string? description)
    {
        return new ExpenseItem(categoryId, itemAmount, description);
    }

    public void Update(Guid categoryId, decimal itemAmount, string? description)
    {
        CategoryId = categoryId;
        ItemAmount = itemAmount;
        Description = description;
    }
}
