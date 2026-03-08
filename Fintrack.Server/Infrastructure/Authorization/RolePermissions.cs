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
                Permissions.InvoicesRead, Permissions.InvoicesWrite, Permissions.InvoicesDelete,
                Permissions.BudgetsRead, Permissions.BudgetsWrite
            },
            [Roles.User] = new HashSet<string>
            {
                Permissions.CategoriesRead, // Built in system categories
                Permissions.ExpensesRead, Permissions.ExpensesWrite, Permissions.ExpensesDelete,
                Permissions.InvoicesRead, Permissions.InvoicesWrite, Permissions.InvoicesDelete,
                Permissions.BudgetsRead, Permissions.BudgetsWrite
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
