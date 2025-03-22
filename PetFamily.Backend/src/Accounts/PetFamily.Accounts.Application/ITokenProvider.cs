using PetFamily.Accounts.Domain.DataModels;

namespace PetFamily.Accounts.Application;

public interface ITokenProvider
{
     Task<string> GenerateAccessToken(User user);
}