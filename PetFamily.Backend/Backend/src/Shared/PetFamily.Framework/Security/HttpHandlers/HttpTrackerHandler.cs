using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PetFamily.Framework.Security.Options;

namespace PetFamily.Framework.Security.HttpHandlers;

public class HttpTrackerHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _context;
    private readonly AuthOptions _authOptions;

    public HttpTrackerHandler(IHttpContextAccessor context, IOptions<AuthOptions> authOptions)
    {
        _context = context;
        _authOptions = authOptions.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        const string authHeader = "Authorization";
        const string interserviceAuthHeader = "X-Internal-Service-Key";

        if (_context.HttpContext == null ||
            !_context.HttpContext.Request.Headers.TryGetValue(authHeader, out var jwtValues) ||
            string.IsNullOrWhiteSpace(jwtValues.FirstOrDefault()))
        {
            request.Headers.Add(interserviceAuthHeader, _authOptions.ServicesKey);
        }
        else
        {
            request.Headers.Add(authHeader, jwtValues.First());
        }

        return base.SendAsync(request, cancellationToken);
    }
}