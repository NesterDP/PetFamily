namespace NotificationService.Contracts.Requests;

public record UpdateUserSettingsRequest(Guid UserId, bool? SendEmail, bool? SendTelegram, bool SendWebsite);