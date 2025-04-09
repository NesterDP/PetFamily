using CSharpFunctionalExtensions;
using PetFamily.Discussions.Application.Commands.CreateDiscussion;
using PetFamily.Discussions.Contracts;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Presentation.Contracts;

public class CreateDiscussionContract : ICreateDiscussionContract
{
    private readonly CreateDiscussionHandler _handler;

    public CreateDiscussionContract(CreateDiscussionHandler handler)
    {
        _handler = handler;
    }

    public async Task<Result<Guid, ErrorList>> CreateDiscussion(
        CreateDiscussionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateDiscussionCommand(request.RelationId, request.UserIds);
        var result = await _handler.HandleAsync(command, cancellationToken);
        return result;
    }
}