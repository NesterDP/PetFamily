using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.API.Response;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.GetVolunteer;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices] IValidator<CreateVolunteerRequest> validator,
        [FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var firstValidLayer = await validator.ValidateAsync(request, cancellationToken);
        if (firstValidLayer.IsValid == false)
            return firstValidLayer.ToValidationErrorResponse();

        var secondValidLayer = await handler.HandleAsync(request, cancellationToken);
        return secondValidLayer.IsFailure ? secondValidLayer.Error.ToResponse() : secondValidLayer.ToResponse();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<string>> Get(
        [FromRoute] Guid id,
        [FromServices] GetVolunteerHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(VolunteerId.Create(id), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return "такой есть, его email = " + result.Value.Email.Value;
    }
}