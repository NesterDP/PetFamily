using System.Security.Claims;
using CSharpFunctionalExtensions;
using PetFamily.Core;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Framework;

public class UserScopedData
{
    public Guid UserId { get; private set; }
    public List<string> Roles { get; } = [];

    public void LoadFromClaims(IEnumerable<Claim> claims)
    {
        var claimId = claims.FirstOrDefault(c => c.Type == CustomClaims.Id);
        
        if (claimId != null && Guid.TryParse(claimId.Value, out var userId))
            UserId = userId;
        else
            UserId = Guid.Empty;
        
        Roles.AddRange(claims
            .Where(c => c.Type == CustomClaims.Role)
            .Select(c => c.Value));
    }
    
    public UnitResult<Error> ConfirmRoleExlusivity(string roleName)
    {
        if (Roles.Any(r => r == roleName) && Roles.Count == 1)
            return UnitResult.Success<Error>();

        return Errors.General
            .Conflict($"user should not have any other role then {roleName} in order to use this feature");
    }
}