using Microsoft.AspNetCore.Authentication;

namespace NotificationService.Security.Options;

public class InterserviceAuthenticationOptions : AuthenticationSchemeOptions
{
    public string HeaderName { get; init; } = "X-Internal-Service-Key";

    public string ExpectedKey { get; set; } = string.Empty;
}