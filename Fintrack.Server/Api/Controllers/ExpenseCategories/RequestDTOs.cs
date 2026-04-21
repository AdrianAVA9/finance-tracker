namespace Fintrack.Server.Api.Controllers.ExpenseCategories
{
    public sealed class RequestCreateExpenseCategory
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public string? Icon { get; init; }
        public string? Color { get; init; }
        public Guid? GroupId { get; init; }
    }

    public sealed class RequestUpdateExpenseCategory
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public string? Icon { get; init; }
        public string? Color { get; init; }
        public Guid? GroupId { get; init; }
    }
}
