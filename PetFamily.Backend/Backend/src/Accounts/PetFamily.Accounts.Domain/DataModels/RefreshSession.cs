namespace PetFamily.Accounts.Domain.DataModels;

public class RefreshSession
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public Guid RefreshToken { get; init; }

    public Guid Jti { get; init; } // identifier of access token

    public DateTime ExpiresIn { get; init; }

    public DateTime CreatedAt { get; init; }
}