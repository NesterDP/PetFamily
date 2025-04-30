using System.Security.Claims;
using System.Text.Encodings.Web;
using FileService.Security.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FileService.Security.Authentication;

public class InterserviceAuthenticationHandler : AuthenticationHandler<InterserviceAuthenticationOptions>
{
    public InterserviceAuthenticationHandler(
        IOptionsMonitor<InterserviceAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var receivedKey))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (receivedKey != Options.ExpectedKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid service key"));
        }

        var claimsIdentity = new ClaimsIdentity("ServiceIdentity");
        claimsIdentity.AddClaim(new Claim("IsService", "true"));
        var principal = new ClaimsPrincipal(claimsIdentity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}