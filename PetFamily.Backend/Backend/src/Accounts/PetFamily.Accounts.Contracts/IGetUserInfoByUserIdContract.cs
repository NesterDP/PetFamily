using CSharpFunctionalExtensions;
using PetFamily.Accounts.Contracts.Dto;
using PetFamily.Accounts.Contracts.Requests;

namespace PetFamily.Accounts.Contracts;

public interface IGetUserInfoByUserIdContract
{
    public Task<Result<UserInfoDto, string>> GetUserInfo(
        GetUserInfoByUserIdRequest userId,
        CancellationToken cancellationToken = default);
}