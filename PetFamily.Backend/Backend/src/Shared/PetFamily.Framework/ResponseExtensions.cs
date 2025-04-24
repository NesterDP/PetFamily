using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Core.Models;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Framework;

public static class ResponseExtensions
{
    public static ActionResult ToResponse(this Error error)
    {
        int statusCode = GetStatusCodeForErrorType(error.Type);

        var envelope = Envelope.Error(error.ToErrorList());

        return new ObjectResult(envelope)
        {
            StatusCode = statusCode,
        };
    }

    public static ActionResult ToResponse(this ErrorList errors)
    {
        if (!errors.Any())
        {
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        var distinctErrorTypes = errors
            .Select(x => x.Type)
            .Distinct()
            .ToList();

        int statusCode = distinctErrorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeForErrorType(distinctErrorTypes.First());

        var envelope = Envelope.Error(errors);

        return new ObjectResult(envelope)
        {
            StatusCode = statusCode,
        };
    }

    public static ActionResult ToResponse<T>(this Result<T, ErrorList> result)
    {
        const int statusCode = StatusCodes.Status200OK;

        var envelope = Envelope.Ok(result.Value);

        return new ObjectResult(envelope)
        {
            StatusCode = statusCode,
        };
    }

    private static int GetStatusCodeForErrorType(ErrorType errorType) =>
     errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Failure => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
}