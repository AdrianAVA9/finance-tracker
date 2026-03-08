using Microsoft.AspNetCore.Authorization;

namespace Fintrack.Server.Infrastructure.Authorization
{
    public sealed class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
