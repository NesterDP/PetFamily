using CSharpFunctionalExtensions;
using PetFamily.Accounts.Contracts.Requests;

namespace PetFamily.Accounts.Contracts;

public interface ICreateVolunteerAccountContract
{
    public Task<Result<Guid, string>> CreateVolunteerAccountAsync(
        CreateVolunteerAccountRequest request,
        CancellationToken cancellationToken = default);
}