using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Fintrack.Server.Infrastructure.Authorization
{
    internal sealed class RoleToPermissionClaimsTransformation 
        : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var roles = principal
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (!roles.Any())
            {
                return Task.FromResult(principal);
            }

            var permissions = RolePermissions.GetPermissionsForRoles(roles);

            var identity = principal.Identity as ClaimsIdentity;

            if (identity is null)
            {
                return Task.FromResult(principal);
            }

            foreach (var permission in permissions)
            {
                if (!principal.HasClaim("permission", permission))
                {
                    identity.AddClaim(new Claim("permission", permission));
                }
            }

            return Task.FromResult(principal);
        }
    }
}
