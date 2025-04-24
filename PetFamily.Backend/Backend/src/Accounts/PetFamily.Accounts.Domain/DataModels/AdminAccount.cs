namespace PetFamily.Accounts.Domain.DataModels;

public class AdminAccount
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; } // navigation

    public User User { get; init; } = null!; // navigation

    public AdminAccount(User user)
    {
        Id = Guid.NewGuid();
        UserId = user.Id;
        User = user;
    }

    private AdminAccount() { } // ef core
}
