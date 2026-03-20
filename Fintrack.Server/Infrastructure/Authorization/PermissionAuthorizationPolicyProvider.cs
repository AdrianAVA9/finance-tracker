using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Fintrack.Server.Infrastructure.Authorization
{
    internal sealed class PermissionAuthorizationPolicyProvider 
        : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public PermissionAuthorizationPolicyProvider(
            IOptions<AuthorizationOptions> options)
            : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy is not null)
            {
                return policy;
            }

            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            _options.AddPolicy(policyName, policy);

            return policy;
        }
    }
}
