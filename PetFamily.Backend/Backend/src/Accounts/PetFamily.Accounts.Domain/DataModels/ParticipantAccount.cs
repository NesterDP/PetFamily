using PetFamily.Accounts.Contracts.Dto;

namespace PetFamily.Accounts.Domain.DataModels;

public class ParticipantAccount
{
    public Guid Id { get; init; }

    public List<FavoritePetDto> FavoritePets { get; init; } = [];

    public Guid UserId { get; init; } // navigation

    public User User { get; init; } = null!; // navigation

    public ParticipantAccount(User user)
    {
        Id = Guid.NewGuid();
        UserId = user.Id;
        User = user;
    }

    private ParticipantAccount() { } // ef core
}