using CSharpFunctionalExtensions;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Contracts;


public interface ICloseDiscussionContract
{
    public Task<Result<Guid, Error>> CloseDiscussion(
        CloseDiscussionRequest request,
        CancellationToken cancellationToken);
}