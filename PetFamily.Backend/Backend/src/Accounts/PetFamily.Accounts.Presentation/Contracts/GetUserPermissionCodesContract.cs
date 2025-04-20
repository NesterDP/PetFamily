using PetFamily.Accounts.Contracts;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.Accounts.Infrastructure.EntityManagers;

namespace PetFamily.Accounts.Presentation.Contracts;

public class GetUserPermissionCodesContract : IGetUserPermissionCodesContract
{
    private readonly PermissionManager _permissionManager;

    public GetUserPermissionCodesContract(PermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    public async Task<HashSet<string>> GetUserPermissionCodes(GetUserPermissionCodesRequest request)
    {
        return await _permissionManager.GetUserPermissionCodes(request.UserId);
    }
}