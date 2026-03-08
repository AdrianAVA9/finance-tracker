namespace Fintrack.Server.Infrastructure.Authorization
{
    public static class Permissions
    {
        // ═══════════════════════════════════════════════════════════════
        // CATEGORY PERMISSIONS
        // ═══════════════════════════════════════════════════════════════
        public const string CategoriesRead = "categories:read";
        public const string CategoriesWrite = "categories:write";
        public const string CategoriesDelete = "categories:delete";

        // ═══════════════════════════════════════════════════════════════
        // EXPENSE PERMISSIONS
        // ═══════════════════════════════════════════════════════════════
        public const string ExpensesRead = "expenses:read";
        public const string ExpensesWrite = "expenses:write";
        public const string ExpensesDelete = "expenses:delete";

        // ═══════════════════════════════════════════════════════════════
        // INVOICE PERMISSIONS
        // ═══════════════════════════════════════════════════════════════
        public const string InvoicesRead = "invoices:read";
        public const string InvoicesWrite = "invoices:write";
        public const string InvoicesDelete = "invoices:delete";

        // ═══════════════════════════════════════════════════════════════
        // BUDGET PERMISSIONS
        // ═══════════════════════════════════════════════════════════════
        public const string BudgetsRead = "budgets:read";
        public const string BudgetsWrite = "budgets:write";
    }
}
