using CSharpFunctionalExtensions;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Contracts;

public interface ICreateVolunteerAccountContract
{
    public Task<Result<Guid, Error>> CreateVolunteerAccountAsync(
        CreateVolunteerAccountRequest request,
        CancellationToken cancellationToken = default);
}