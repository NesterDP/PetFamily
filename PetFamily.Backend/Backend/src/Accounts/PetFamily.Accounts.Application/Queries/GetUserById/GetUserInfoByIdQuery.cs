using PetFamily.Core.Abstractions;

namespace PetFamily.Accounts.Application.Queries.GetUserById;

public record GetUserInfoByIdQuery(Guid UserId) : IQuery;