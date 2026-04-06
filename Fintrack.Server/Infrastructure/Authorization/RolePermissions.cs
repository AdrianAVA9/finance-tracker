namespace Fintrack.Server.Infrastructure.Authorization
{
    public static class RolePermissions
    {
        private static readonly Dictionary<string, HashSet<string>> RolePermissionMap = new()
        {
            [Roles.Admin] = new HashSet<string>
            {
                Permissions.CategoriesRead, Permissions.CategoriesWrite, Permissions.CategoriesDelete,
                Permissions.ExpensesRead, Permissions.ExpensesWrite, Permissions.ExpensesDelete,
                Permissions.IncomesRead, Permissions.IncomesWrite, Permissions.IncomesDelete,
                Permissions.InvoicesRead, Permissions.InvoicesWrite, Permissions.InvoicesDelete,
                Permissions.BudgetsRead, Permissions.BudgetsWrite
            },
            [Roles.User] = new HashSet<string>
            {
                Permissions.CategoriesRead, // Built in system categories
                Permissions.ExpensesRead, Permissions.ExpensesWrite, Permissions.ExpensesDelete,
                Permissions.IncomesRead, Permissions.IncomesWrite, Permissions.IncomesDelete,
                Permissions.InvoicesRead, Permissions.InvoicesWrite, Permissions.InvoicesDelete,
                Permissions.BudgetsRead, Permissions.BudgetsWrite
            },
            // Authenticated user with no budget permissions (integration / authZ tests only)
            [Roles.IntegrationTestNoBudgets] = new HashSet<string>
            {
                Permissions.CategoriesRead
            },
            [Roles.IntegrationTestBudgetReadOnly] = new HashSet<string>
            {
                Permissions.CategoriesRead,
                Permissions.BudgetsRead
            },
            [Roles.IntegrationTestNoExpenses] = new HashSet<string>
            {
                Permissions.CategoriesRead
            },
            [Roles.IntegrationTestExpenseReadOnly] = new HashSet<string>
            {
                Permissions.CategoriesRead,
                Permissions.ExpensesRead
            },
            [Roles.IntegrationTestNoIncomes] = new HashSet<string>
            {
                Permissions.CategoriesRead
            },
            [Roles.IntegrationTestIncomeReadOnly] = new HashSet<string>
            {
                Permissions.CategoriesRead,
                Permissions.IncomesRead
            }
        };

        public static IReadOnlySet<string> GetPermissionsForRole(string role)
        {
            return RolePermissionMap.TryGetValue(role, out var permissions)
                ? permissions
                : new HashSet<string>();
        }

        public static IReadOnlySet<string> GetPermissionsForRoles(IEnumerable<string> roles)
        {
            var allPermissions = new HashSet<string>();
            foreach (var role in roles)
            {
                var permissions = GetPermissionsForRole(role);
                allPermissions.UnionWith(permissions);
            }
            return allPermissions;
        }

        public static bool HasPermission(string role, string permission)
        {
            return RolePermissionMap.TryGetValue(role, out var permissions)
                   && permissions.Contains(permission);
        }
    }
}
