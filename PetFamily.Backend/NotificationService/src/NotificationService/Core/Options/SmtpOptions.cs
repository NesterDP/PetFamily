namespace NotificationService.Core.Options;

public class SmtpOptions
{
    public const string SMTP_OPTIONS = "SmtpOptions"; 
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    
    public bool UseSsl { get; set; }
    
    public string FromEmail { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}