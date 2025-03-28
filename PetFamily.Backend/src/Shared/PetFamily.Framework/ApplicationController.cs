using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Core;
using PetFamily.Core.Models;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Framework;

[ApiController]
[Route("[controller]")]
public abstract class ApplicationController : ControllerBase
{
    public override OkObjectResult Ok(object? value)
    {
        var envelope = Envelope.Ok(value);
        
        return new OkObjectResult(envelope);
    }

    protected Result<Guid, Error> GetUserId()
    {
        var claimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaims.Id);
        if (claimId == null)
            return Errors.General.ValueNotFound("claimId");

        var parseResult = Guid.TryParse(claimId.Value, out var userId);
        if (!parseResult)
            return Errors.General.ValueIsInvalid("userId");
        
        return userId;
    }

    protected UnitResult<Error> CheckExclusiveRole(string roleName)
    {
        var roles = HttpContext.User.Claims.Where(c => c.Type == CustomClaims.Role);
        if (roles.Any(r => r.Value == roleName) && roles.Count() == 1)
            return UnitResult.Success<Error>();

        return Errors.General
            .Conflict($"user should not have any other role then {roleName} in order to use this feature");
    }
}