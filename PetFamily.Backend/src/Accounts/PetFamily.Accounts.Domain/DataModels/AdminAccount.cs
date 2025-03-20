namespace PetFamily.Accounts.Domain.DataModels;

public class AdminAccount
{
    public const string ADMIN = "Admin";
    
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } // navigation

    public AdminAccount(User user)
    {
        Id = Guid.NewGuid();
        User = user;
    }

    private AdminAccount() { } // ef core
}