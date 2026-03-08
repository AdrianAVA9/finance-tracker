namespace Fintrack.Server.Controllers.ExpenseCategories
{
    public sealed class RequestCreateExpenseCategory
    {
        public required string Name { get; init; }
        public string? Icon { get; init; }
        public string? Color { get; init; }
    }

    public sealed class RequestUpdateExpenseCategory
    {
        public required string Name { get; init; }
        public string? Icon { get; init; }
        public string? Color { get; init; }
    }
}
