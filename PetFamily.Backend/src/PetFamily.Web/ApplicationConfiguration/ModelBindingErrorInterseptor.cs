using Microsoft.AspNetCore.Mvc;
using PetFamily.Core.Models;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Web.ApplicationConfiguration;

public static class ModelBindingErrorInterceptor
{
    public static IMvcBuilder InterceptModelBindingError(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var responseError = Errors.General.Failure(
                        "model.binding.error", "failed to bind the received model");
                    return new BadRequestObjectResult(Envelope.Error(responseError.ToErrorList()));
                };
            });
        return builder;
    }
}