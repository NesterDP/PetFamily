using CSharpFunctionalExtensions;
using PetFamily.Accounts.Application.Queries.GetUserById;
using PetFamily.Accounts.Contracts;
using PetFamily.Accounts.Contracts.Dto;
using PetFamily.Accounts.Contracts.Requests;

namespace PetFamily.Accounts.Presentation.Contracts;

public class GetUserInfoByUserIdContract : IGetUserInfoByUserIdContract
{
    private readonly GetUserInfoByIdHandler _handler;

    public GetUserInfoByUserIdContract(GetUserInfoByIdHandler handler)
    {
        _handler = handler;
    }

    public async Task<Result<UserInfoDto, string>> GetUserInfo(
        GetUserInfoByUserIdRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserInfoByIdQuery(request.UserId);

        var result = await _handler.HandleAsync(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.FirstOrDefault()!.Message;

        return result.Value;
    }
}