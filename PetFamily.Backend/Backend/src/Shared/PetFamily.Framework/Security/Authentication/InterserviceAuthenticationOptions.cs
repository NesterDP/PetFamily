﻿using Microsoft.AspNetCore.Authentication;

namespace PetFamily.Framework.Security.Authentication;

public class InterserviceAuthenticationOptions : AuthenticationSchemeOptions
{
    public string HeaderName { get; init; } = "X-Internal-Service-Key";

    public string ExpectedKey { get; set; } = string.Empty;
}