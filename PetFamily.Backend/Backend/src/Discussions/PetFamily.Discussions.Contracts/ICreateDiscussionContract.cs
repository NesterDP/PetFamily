using CSharpFunctionalExtensions;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Contracts;

public interface ICreateDiscussionContract
{
    public Task<Result<Guid, ErrorList>> CreateDiscussion(
        CreateDiscussionRequest request,
        CancellationToken cancellationToken);
}