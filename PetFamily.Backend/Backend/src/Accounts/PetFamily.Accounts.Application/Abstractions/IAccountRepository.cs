using CSharpFunctionalExtensions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Abstractions;

public interface IAccountRepository
{
    Task<Result<User, Error>> GetUserById(Guid id);

    Task<User?> GetNullableUserById(Guid id);
}