using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Fintrack.Server.Infrastructure.Authorization
{
    internal sealed class RoleToPermissionClaimsTransformation 
        : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // Do not mutate the original principal. Clone it.
            var clone = principal.Clone();

            var roles = clone
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            // Default fallback: if the user has no explicit roles assigned
            if (!roles.Any())
            {
                roles.Add(Roles.User);
            }

            var permissions = RolePermissions.GetPermissionsForRoles(roles);

            var newIdentity = new ClaimsIdentity();
            var addedAny = false;

            foreach (var permission in permissions)
            {
                if (!clone.HasClaim("permission", permission))
                {
                    newIdentity.AddClaim(new Claim("permission", permission));
                    addedAny = true;
                }
            }

            if (addedAny)
            {
                clone.AddIdentity(newIdentity);
            }

            return Task.FromResult(clone);
        }
    }
}
