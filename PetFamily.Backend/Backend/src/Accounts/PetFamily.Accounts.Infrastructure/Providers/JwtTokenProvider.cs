using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Application.Models;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Core;
using PetFamily.Core.Options;
using PetFamily.Framework;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Infrastructure.Providers;

public class JwtTokenProvider : ITokenProvider
{
    private readonly RefreshSessionOptions _refreshSessionOptions;
    private readonly AccountsDbContext _dbContext;
    private readonly JwtOptions _jwtOptions;

    public JwtTokenProvider(
        IOptions<JwtOptions> jwtOptions,
        IOptions<RefreshSessionOptions> refreshSessionOptions,
        AccountsDbContext dbContext)
    {
        _jwtOptions = jwtOptions.Value;
        _refreshSessionOptions = refreshSessionOptions.Value;
        _dbContext = dbContext;
    }

    public async Task<JwtTokenResult> GenerateAccessToken(User user, CancellationToken cancellationToken)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roleClaims = await _dbContext.Users
            .Include(u => u.Roles) // surpass lazy loading
            .Where(u => u.Id == user.Id)
            .SelectMany(u => u.Roles)
            .Select(r => new Claim(CustomClaims.ROLE, r.Name!))
            .ToListAsync(cancellationToken);

        var jti = Guid.NewGuid();

        var claims = new[]
        {
            new Claim(CustomClaims.ID, user.Id.ToString()), new Claim(CustomClaims.JTI, jti.ToString()),
            new Claim(CustomClaims.EMAIL, user.Email ?? string.Empty),
        };

        claims = claims.Concat(roleClaims).ToArray();

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_jwtOptions.ExpiredMinutesTime)),
            signingCredentials: signingCredentials,
            claims: claims);

        string? accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return new JwtTokenResult(accessToken, jti);
    }

    public async Task<Guid> GenerateRefreshToken(User user, Guid accessTokenJti, CancellationToken cancellationToken)
    {
        var refreshSession = new RefreshSession
        {
            User = user,
            CreatedAt = DateTime.UtcNow,
            ExpiresIn = DateTime.UtcNow.AddDays(int.Parse(_refreshSessionOptions.ExpiredDaysTime)),
            Jti = accessTokenJti,
            RefreshToken = Guid.NewGuid(),
        };

        await _dbContext.AddAsync(refreshSession, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return refreshSession.RefreshToken;
    }

    public async Task<Result<IReadOnlyList<Claim>, Error>> GetUserClaims(string jwtToken)
    {
        var jwtHandler = new JwtSecurityTokenHandler();

        var validationParameters = TokenValidationParametersFactory.CreateWithoutLifeTime(_jwtOptions);

        var validationResult = await jwtHandler.ValidateTokenAsync(jwtToken, validationParameters);

        if (validationResult.IsValid == false)
            return Errors.General.InvalidToken();

        return validationResult.ClaimsIdentity.Claims.ToList();
    }
}