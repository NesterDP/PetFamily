using CSharpFunctionalExtensions;
using PetFamily.Accounts.Contracts.Dto;
using PetFamily.Accounts.Contracts.Requests;

namespace PetFamily.Accounts.Communication;

public interface IAccountsService
{
    Task<Result<UserInfoDto, string>> GetUserInfoById(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result<string, string>> GenerateEmailConfirmationToken(
        GenerateEmailTokenRequest request,
        CancellationToken cancellationToken);
}