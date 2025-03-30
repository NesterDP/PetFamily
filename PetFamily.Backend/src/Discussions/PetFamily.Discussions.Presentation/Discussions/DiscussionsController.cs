using Microsoft.AspNetCore.Mvc;
using PetFamily.Discussions.Application.Commands.CloseDiscussion;
using PetFamily.Discussions.Application.Queries.GetDiscussion;
using PetFamily.Framework;
using PetFamily.Framework.Authorization;

namespace PetFamily.Discussions.Presentation.Discussions;

public class DiscussionsController : ApplicationController
{
    [Permission("discussions.CloseDiscussion")]
    [HttpPut("{id:guid}/close-discussion")]
    public async Task<ActionResult<Guid>> CloseDiscussion(
        [FromRoute] Guid id,
        [FromServices] CloseDiscussionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CloseDiscussionCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [Permission("discussions.GetDiscussion")]
    [HttpGet("{id:guid}/discussion")]
    public async Task<ActionResult<Guid>> GetDiscussion(
        [FromRoute] Guid id,
        [FromServices] GetDiscussionHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetDiscussionQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
}