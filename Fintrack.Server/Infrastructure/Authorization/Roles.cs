namespace Fintrack.Server.Infrastructure.Authorization
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";

        /// <summary>
        /// Used by integration tests to exercise authorization without budget permissions.
        /// Not assigned to real users in production flows.
        /// </summary>
        public const string IntegrationTestNoBudgets = "IntegrationTest.NoBudgets";

        /// <summary>
        /// Integration tests: can read budgets but not mutate them.
        /// </summary>
        public const string IntegrationTestBudgetReadOnly = "IntegrationTest.BudgetReadOnly";

        /// <summary>
        /// Integration tests: authenticated but has no expense permissions.
        /// </summary>
        public const string IntegrationTestNoExpenses = "IntegrationTest.NoExpenses";

        /// <summary>
        /// Integration tests: can read expenses but not create, update, or delete them.
        /// </summary>
        public const string IntegrationTestExpenseReadOnly = "IntegrationTest.ExpenseReadOnly";

        /// <summary>
        /// Integration tests: authenticated but has no income permissions.
        /// </summary>
        public const string IntegrationTestNoIncomes = "IntegrationTest.NoIncomes";

        /// <summary>
        /// Integration tests: can read incomes but not create, update, or delete them.
        /// </summary>
        public const string IntegrationTestIncomeReadOnly = "IntegrationTest.IncomeReadOnly";
    }
}
