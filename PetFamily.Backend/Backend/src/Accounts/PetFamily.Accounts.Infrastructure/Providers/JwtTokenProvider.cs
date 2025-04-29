using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Application.Models;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Core;
using PetFamily.Core.Caching;
using PetFamily.Core.Options;
using PetFamily.Framework.Security.Authentication;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Infrastructure.Providers;

public class JwtTokenProvider : ITokenProvider
{
    private readonly RefreshSessionOptions _refreshSessionOptions;
    private readonly ICacheService _cacheService;
    private readonly AccountsDbContext _dbContext;
    private readonly AuthOptions _authOptions;

    public JwtTokenProvider(
        IOptions<AuthOptions> jwtOptions,
        IOptions<RefreshSessionOptions> refreshSessionOptions,
        ICacheService cacheService,
        AccountsDbContext dbContext)
    {
        _authOptions = jwtOptions.Value;
        _refreshSessionOptions = refreshSessionOptions.Value;
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<JwtTokenResult> GenerateAccessToken(User user, CancellationToken cancellationToken)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.PrivateKey));
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
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_authOptions.ExpiredMinutesTime)),
            signingCredentials: signingCredentials,
            claims: claims);

        string? accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return new JwtTokenResult(accessToken, jti);
    }

    public async Task<Guid> GenerateRefreshToken(User user, Guid accessTokenJti, CancellationToken cancellationToken)
    {
        var refreshSession = new RefreshSession
        {
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresIn = DateTime.UtcNow.AddDays(int.Parse(_refreshSessionOptions.ExpiredDaysTime)),
            Jti = accessTokenJti,
            RefreshToken = Guid.NewGuid(),
        };

        string key = CacheConstants.REFRESH_SESSIONS_PREFIX + refreshSession.RefreshToken;
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = refreshSession.ExpiresIn // Автоматическое удаление по истечении срока
        };

        await _cacheService.SetAsync(key, refreshSession, options, cancellationToken);

        return refreshSession.RefreshToken;
    }

    public async Task<Result<IReadOnlyList<Claim>, Error>> GetUserClaims(string jwtToken)
    {
        var jwtHandler = new JwtSecurityTokenHandler();

        var validationParameters = TokenValidationParametersFactory.CreateWithoutLifeTime(_authOptions);

        var validationResult = await jwtHandler.ValidateTokenAsync(jwtToken, validationParameters);

        if (validationResult.IsValid == false)
            return Errors.General.InvalidToken();

        return validationResult.ClaimsIdentity.Claims.ToList();
    }
}