using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Discussions.Application.Abstractions;
using PetFamily.Discussions.Application.Commands.CloseDiscussion;
using PetFamily.Discussions.Contracts;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;

namespace PetFamily.Discussions.Presentation.Contracts;

public class CloseDiscussionContract : ICloseDiscussionContract
{
    private readonly CloseDiscussionHandler _handler;

    public CloseDiscussionContract(
        [FromServices] CloseDiscussionHandler handler)
    {
        _handler = handler;
    }
    public async Task<Result<Guid, ErrorList>> CloseDiscussion(
        CloseDiscussionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CloseDiscussionCommand(request.RelationId, request.UserId);
        var result = await _handler.HandleAsync(command, cancellationToken);
        return result;
    }
}