using PetFamily.Accounts.Contracts.Requests;

namespace PetFamily.Accounts.Contracts;

public interface IGetUserPermissionCodesContract
{
    Task<HashSet<string>> GetUserPermissionCodes(GetUserPermissionCodesRequest request);
}