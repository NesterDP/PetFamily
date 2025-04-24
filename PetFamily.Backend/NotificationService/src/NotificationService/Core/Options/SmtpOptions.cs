namespace NotificationService.Core.Options;

public class SmtpOptions
{
    public const string SMTP_OPTIONS = "SmtpOptions";

    public string Host { get; init; } = string.Empty;

    public int Port { get; init; }

    public bool UseSsl { get; init; }

    public string FromEmail { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}