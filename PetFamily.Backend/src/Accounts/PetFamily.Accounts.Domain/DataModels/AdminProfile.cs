namespace PetFamily.Accounts.Domain.DataModels;

public class AdminProfile
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; } // navigation
}