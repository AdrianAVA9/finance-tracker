namespace Fintrack.Server.Api.Controllers.ExpenseCategoryGroups
{
    public sealed class RequestCreateExpenseCategoryGroup
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
    }

    public sealed class RequestUpdateExpenseCategoryGroup
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
    }
}
