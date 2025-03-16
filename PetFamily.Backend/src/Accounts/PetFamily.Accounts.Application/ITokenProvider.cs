using PetFamily.Accounts.Domain.DataModels;

namespace PetFamily.Accounts.Application;

public interface ITokenProvider
{
     string GenerateAccessToken(User user);
}