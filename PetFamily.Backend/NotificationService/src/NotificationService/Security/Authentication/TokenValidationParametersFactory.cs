using System.Text;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Security.Options;

namespace NotificationService.Security.Authentication;

public static class TokenValidationParametersFactory
{
    public static TokenValidationParameters CreateWithLifeTime(AuthOptions authOptions) =>
        new()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
        };

    /*public static TokenValidationParameters CreateWithoutLifeTime(AuthOptions authOptions) =>
        new()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
        };*/
}