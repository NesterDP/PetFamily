using FileService.Shared.CustomErrors;
using FileService.Shared.Models;

namespace FileService.API;

public static class CustomResponses
{
    public static IResult EnvelopedOk(object? value)
    {
        var envelope = Envelope.Ok(value);
        
        return Results.Ok(envelope);
    }
    
    public static IResult EnvelopedErrors(ErrorList errorList)
    {
        var envelope = Envelope.Error(errorList);
        
        return Results.Ok(envelope);
    }
}