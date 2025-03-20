namespace PetFamily.Core.Options;

public class AdminOptions
{
    public const string ADMIN = nameof(ADMIN);

    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}