using NotificationService.DataModels.ValueObjects;

namespace NotificationService.DataModels;

public class UserNotificationSettings
{
    public UserNotificationSettingsId Id { get; private set; }
    public UserId UserId { get; private set; }
    public bool SendToEmail { get; private set; }
    public bool SendToTelegram { get; private set; }
    public bool SendToWebsite { get; private set; }

    // ef core
    public UserNotificationSettings() { }

    public UserNotificationSettings(
        UserId userId,
        bool sendToEmail,
        bool sendToTelegram,
        bool sendToWebsite)
    {
        Id = UserNotificationSettingsId.NewId();
        UserId = userId;
        SendToEmail = sendToEmail;
        SendToTelegram = sendToTelegram;
        SendToWebsite = sendToWebsite;
    }

    public void SetUserId(UserId userId)
    {
        UserId = userId;
    }

    public void SetEmailNotification(bool sendToEmail)
    {
        SendToEmail = sendToEmail;
    }

    public void SetTelegramNotification(bool sendToTelegram)
    {
        SendToTelegram = sendToTelegram;
    }

    public void SendWebSiteNotification(bool sendToWebsite)
    {
        SendToWebsite = sendToWebsite;
    }
}