using FileService.Core.CustomErrors;
using FileService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileService.ApplicationConfiguration;

public static class ModelBindingErrorInterceptor
{
    public static IMvcBuilder InterceptModelBindingError(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(
            options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.InvalidModelStateResponseFactory = _ =>
                {
                    var responseError = Errors.General.Failure(
                        "model.binding.error", "failed to bind the received model");
                    return new BadRequestObjectResult(Envelope.Error(responseError.ToErrorList()));
                };
            });
        return builder;
    }
}