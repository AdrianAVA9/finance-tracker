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
    }
}
