using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Fintrack.Server.Infrastructure.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionAuthorization(
            this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, 
                PermissionAuthorizationPolicyProvider>();

            services.AddScoped<IAuthorizationHandler, 
                PermissionAuthorizationHandler>();

            services.AddScoped<IClaimsTransformation, 
                RoleToPermissionClaimsTransformation>();

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.FallbackPolicy = null;
            });

            return services;
        }
    }
}
