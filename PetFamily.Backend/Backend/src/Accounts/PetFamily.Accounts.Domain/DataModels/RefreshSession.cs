namespace PetFamily.Accounts.Domain.DataModels;

public class RefreshSession
{
    public Guid RefreshToken { get; init; }

    public Guid UserId { get; init; }

    public Guid Jti { get; init; } // identifier of access token

    public DateTime ExpiresIn { get; init; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime CreatedAt { get; init; }
}