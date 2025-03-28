namespace PetFamily.Accounts.Domain.DataModels;

public class AdminAccount
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; } // navigation
    public User User { get; set; } // navigation

    public AdminAccount(User user)
    {
        Id = Guid.NewGuid();
        UserId = user.Id;
        User = user;
    }

    private AdminAccount() { } // ef core
}