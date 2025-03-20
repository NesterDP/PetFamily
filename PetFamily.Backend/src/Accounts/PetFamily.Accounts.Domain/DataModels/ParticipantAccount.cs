using PetFamily.Core.Dto.Pet;

namespace PetFamily.Accounts.Domain.DataModels;

public class ParticipantAccount
{
    public const string PARTICIPANT = "Participant";
    
    public Guid Id { get; set; }
    
    public List<FavoritePetDto> FavoritePets { get; set; } = [];
    
    public Guid UserId { get; set; }

    public User User { get; set; } // navigation
    
    private ParticipantAccount() { } // ef core
    public ParticipantAccount(User user)
    {
        Id = Guid.NewGuid();
        User = user;
    }
   
}