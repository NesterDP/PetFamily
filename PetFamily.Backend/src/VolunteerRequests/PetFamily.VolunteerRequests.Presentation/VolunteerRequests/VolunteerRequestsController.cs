using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Authorization;
using PetFamily.SharedKernel.Constants;
using PetFamily.VolunteerRequests.Application.Commands.CreateVolunteerRequest;
using PetFamily.VolunteerRequests.Presentation.VolunteerRequests.Requests;

namespace PetFamily.VolunteerRequests.Presentation.VolunteerRequests;

public class VolunteerRequestsController : ApplicationController
{
    [Permission("volunteerRequests.CreateRequest")]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateRequest(
        [FromBody] CreateVolunteerRequestRequest request,
        [FromServices] CreateVolunteerRequestHandler handler,
        CancellationToken cancellationToken)
    {
        // users can have multiple roles, this feature is for users that doesn't have any roles but participant role
        var onlyParticipant = CheckExclusiveRole(DomainConstants.PARTICIPANT);
        if (onlyParticipant.IsFailure)
            return onlyParticipant.Error.ToResponse();
        
        var command = new CreateVolunteerRequestCommand(GetUserId().Value, request.VolunteerInfo);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
}