using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Fintrack.Server.Infrastructure.Authorization
{
    internal sealed class PermissionAuthorizationHandler 
        : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(
            ILogger<PermissionAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var permissions = context.User
                .FindAll("permission")
                .Select(c => c.Value)
                .ToHashSet();

            if (permissions.Contains(requirement.Permission))
            {
                _logger.LogDebug(
                    "User {UserId} authorized for permission {Permission}",
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    requirement.Permission);

                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning(
                    "User {UserId} denied permission {Permission}. User has permissions: {Permissions}",
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    requirement.Permission,
                    string.Join(", ", permissions));
            }

            return Task.CompletedTask;
        }
    }
}
