using Microsoft.AspNetCore.Mvc;
using NotificationService.API.Endpoints;
using NotificationService.Contracts.Requests;
using NotificationService.Core.Structs;
using NotificationService.DataModels;
using NotificationService.DataModels.ValueObjects;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.TransactionServices;
using NotificationService.Security.Authorization;

namespace NotificationService.Features;

public static class UpdateNotificationSettings
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("notification-settings", Handler)
                .RequirePermission("notificationservice.UpdateNotificationSettings");
        }
    }

    private static async Task<IResult> Handler(
        [FromBody] UpdateUserSettingsRequest request,
        [FromKeyedServices(UnitOfWorkSelector.UsersNotificationSettings)]
        IUnitOfWork unitOfWork,
        INotificationsRepository repository,
        ILogger<Endpoint> logger)
    {
        var userId = UserId.Create(request.UserId);

        var settings = await repository.GetByUserIdAsync(userId);
        if (settings.IsSuccess)
        {
            settings.Value.SetEmailNotification(request.SendEmail);
            settings.Value.SetTelegramNotification(request.SendTelegram);
            settings.Value.SetWebSiteNotification(request.SendWebsite);
            await unitOfWork.SaveChangesAsync();
            return Results.Ok(settings.Value.Id.Value);
        }

        var userSettings = new UserNotificationSettings(
            userId,
            request.SendEmail,
            request.SendTelegram,
            request.SendWebsite);

        var result = await repository.AddAsync(userSettings);
        await unitOfWork.SaveChangesAsync();
        return Results.Ok(result);
    }
}