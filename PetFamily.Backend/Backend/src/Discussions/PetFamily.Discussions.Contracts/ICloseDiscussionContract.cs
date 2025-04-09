using CSharpFunctionalExtensions;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Contracts;


public interface ICloseDiscussionContract
{
    public Task<Result<Guid, ErrorList>> CloseDiscussion(
        CloseDiscussionRequest request,
        CancellationToken cancellationToken);
}