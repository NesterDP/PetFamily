using NotificationService.Core.CustomErrors;
using NotificationService.Core.Models;

namespace NotificationService.API;

public static class CustomResponses
{
    public static IResult Ok(object? value)
    {
        var envelope = Envelope.Ok(value);
        
        return Results.Ok(envelope);
    }
    
    public static IResult Errors(ErrorList errorList)
    {
        var envelope = Envelope.Error(errorList);
        
        return Results.Conflict(envelope);
    }
}