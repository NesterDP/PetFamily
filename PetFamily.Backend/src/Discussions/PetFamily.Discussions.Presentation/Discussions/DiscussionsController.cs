using Microsoft.AspNetCore.Mvc;
using PetFamily.Discussions.Application.Commands.AddMessage;
using PetFamily.Discussions.Application.Commands.CloseDiscussion;
using PetFamily.Discussions.Application.Commands.RemoveMessage;
using PetFamily.Discussions.Application.Queries.GetDiscussion;
using PetFamily.Discussions.Presentation.Discussions.Requests;
using PetFamily.Framework;
using PetFamily.Framework.Authorization;

namespace PetFamily.Discussions.Presentation.Discussions;

public class DiscussionsController : ApplicationController
{
    [Permission("discussions.CloseDiscussion")]
    [HttpPut("{relationId:guid}")]
    public async Task<ActionResult<Guid>> CloseDiscussion(
        [FromRoute] Guid relationId,
        [FromServices] CloseDiscussionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CloseDiscussionCommand(relationId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [Permission("discussions.GetDiscussion")]
    [HttpGet("{relationId:guid}")]
    public async Task<ActionResult<Guid>> GetDiscussion(
        [FromRoute] Guid relationId,
        [FromServices] GetDiscussionHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetDiscussionQuery(relationId);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [Permission( "discussions.AddMessage")]
    [HttpPost("{relationId:guid}/message")]
    public async Task<ActionResult<Guid>> AddMessage(
        [FromRoute] Guid relationId,
        [FromBody] AddMessageRequest request,
        [FromServices] AddMessageHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(relationId, GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [Permission( "discussions.RemoveMessage")]
    [HttpDelete("{relationId:guid}/message/{messageId:guid}")]
    public async Task<ActionResult<Guid>> AddMessage(
        [FromRoute] Guid relationId,
        [FromRoute] Guid messageId,
        [FromServices] RemoveMessageHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMessageCommand(relationId, messageId, GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
}