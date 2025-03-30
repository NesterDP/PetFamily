using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Authorization;
using PetFamily.SharedKernel.Constants;
using PetFamily.VolunteerRequests.Application.Commands.AmendRequest;
using PetFamily.VolunteerRequests.Application.Commands.ApproveRequest;
using PetFamily.VolunteerRequests.Application.Commands.CreateVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Commands.RejectRequest;
using PetFamily.VolunteerRequests.Application.Commands.RequireRevision;
using PetFamily.VolunteerRequests.Application.Commands.TakeRequestOnReview;
using PetFamily.VolunteerRequests.Application.Queries.GetHandledRequestsByAdminId;
using PetFamily.VolunteerRequests.Application.Queries.GetRequestsByUserId;
using PetFamily.VolunteerRequests.Application.Queries.GetUnhandledRequests;
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

    [Permission("volunteerRequests.TakeRequestOnReview")]
    [HttpPut("{id:guid}/set-on-review")]
    public async Task<ActionResult<Guid>> TakeRequestOnReview(
        [FromRoute] Guid id,
        [FromServices] TakeRequestOnReviewHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new TakeRequestOnReviewCommand(id, GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteerRequests.RequireRevision")]
    [HttpPut("{id:guid}/require-revision")]
    public async Task<ActionResult<Guid>> RequireRevision(
        [FromRoute] Guid id,
        [FromBody] RequireRevisionRequest request,
        [FromServices] RequireRevisionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteerRequests.AmendRequest")]
    [HttpPut("{id:guid}/amend-request")]
    public async Task<ActionResult<Guid>> AmendRequest(
        [FromRoute] Guid id,
        [FromBody] AmendRequestRequest request,
        [FromServices] AmendRequestHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteerRequests.ApproveRequest")]
    [HttpPut("{id:guid}/approve-request")]
    public async Task<ActionResult<Guid>> ApproveRequest(
        [FromRoute] Guid id,
        [FromServices] ApproveRequestHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new ApproveRequestCommand(id, GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteerRequests.RejectRequest")]
    [HttpPut("{id:guid}/reject-request")]
    public async Task<ActionResult<Guid>> RejectRequest(
        [FromRoute] Guid id,
        [FromServices] RejectRequestHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new RejectRequestCommand(id, GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteerRequests.GetUnhandledRequests")]
    [HttpGet("unhandled-requests")]
    public async Task<ActionResult<Guid>> GetUnhandledRequests(
        [FromQuery] GetUnhandledRequestsRequest request,
        [FromServices] GetUnhandledRequestsHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }

    [Permission("volunteerRequests.GetHandledRequestsByAdminId")]
    [HttpGet("handled-requests")]
    public async Task<ActionResult<Guid>> GetHandledRequestsByAdminId(
        [FromQuery] GetHandledRequestsByAdminIdRequest request,
        [FromServices] GetHandledRequestsByAdminIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery(GetUserId().Value);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }

    [Permission("volunteerRequests.GetRequestsByUserId")]
    [HttpGet("requests-by-user")]
    public async Task<ActionResult<Guid>> GetRequestsByUserId(
        [FromQuery] GetRequestsByUserIdRequest request,
        [FromServices] GetRequestsByUserIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery(GetUserId().Value);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
}