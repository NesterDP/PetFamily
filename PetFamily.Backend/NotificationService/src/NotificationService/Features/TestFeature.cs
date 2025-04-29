using NotificationService.API.Endpoints;
using NotificationService.Security.Authorization;
using NotificationService.Services;
using PetFamily.Accounts.Contracts.Dto;

namespace NotificationService.Features;

public static class TestFeature
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("send-email", Handler)
                .RequirePermission("notificationservice.Test");
        }
    }

    private static async Task<IResult> Handler(EmailService emailService)
    {
        var fullNameDto = new FullNameDto("TestFirstName", "TestLastName", "TestSurname");
        string userId = "f82894ed-cef5-4026-9cd5-9094c6610ce5";
        string token = "testToken";

        var userInfo = new UserInfoDto() { Email = "testmail@mail.ru", FullName = fullNameDto };
        await emailService.SendConfirmationEmailAsync(userInfo, userId, token);

        return Results.Ok();
    }
}