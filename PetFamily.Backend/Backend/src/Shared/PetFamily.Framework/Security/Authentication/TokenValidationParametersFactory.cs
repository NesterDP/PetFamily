using System.Text;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Core.Options;

namespace PetFamily.Framework.Security.Authentication;

public static class TokenValidationParametersFactory
{
    public static TokenValidationParameters CreateWithLifeTime(AuthOptions authOptions) =>
        new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.PrivateKey)),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
        };

    public static TokenValidationParameters CreateWithoutLifeTime(AuthOptions authOptions) =>
        new()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
        };
}