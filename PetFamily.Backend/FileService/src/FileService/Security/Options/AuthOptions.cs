namespace FileService.Security.Options;

public class AuthOptions
{
    public const string AUTH = "Auth";

    public string PrivateKey { get; init; } = string.Empty;

    public string ServicesKey { get; init; } = string.Empty;

    public string ExpiredMinutesTime { get; init; } = string.Empty;
}