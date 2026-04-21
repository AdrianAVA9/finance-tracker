using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fintrack.IntegrationTests.Infrastructure
{
    public class TestAuthSchemeOptions : AuthenticationSchemeOptions { }

    public class TestAuthHandler : AuthenticationHandler<TestAuthSchemeOptions>
    {
        public const string SchemeName = "TestScheme";
        public const string TestUserIdHeader = "X-Test-User-Id";
        public const string TestUserPermissionsHeader = "X-Test-User-Permissions";
        public const string TestUserRolesHeader = "X-Test-User-Roles";

        public TestAuthHandler(
            IOptionsMonitor<TestAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(TestUserIdHeader, out var userIdHeader))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userIdHeader.ToString())
            };

            if (Request.Headers.TryGetValue(TestUserRolesHeader, out var rolesHeader))
            {
                foreach (var role in rolesHeader.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (Request.Headers.TryGetValue(TestUserPermissionsHeader, out var permissionsHeader))
            {
                foreach (var permission in permissionsHeader.ToString().Split(','))
                {
                    claims.Add(new Claim("permission", permission.Trim()));
                }
            }

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
