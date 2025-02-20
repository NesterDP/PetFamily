using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.GetVolunteer;
using PetFamily.Application.Volunteers.UpdateMainInfo;
using PetFamily.Application.Volunteers.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.UpdateTransferDetails;
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

    [HttpPut("{id:guid}/main-info")]
    public async Task<ActionResult<Guid>> UpdateMainInfo(
        [FromServices] IValidator<UpdateMainInfoRequest> validator,
        [FromServices] UpdateMainInfoHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoDto dto,
        CancellationToken cancellationToken)
    {
        var request = new UpdateMainInfoRequest(id, dto);
        var firstValidLayer = await validator.ValidateAsync(request, cancellationToken);
        if (firstValidLayer.IsValid == false)
            return firstValidLayer.ToValidationErrorResponse();

        var secondValidLayer = await handler.HandleAsync(request, cancellationToken);
        return secondValidLayer.IsFailure ? secondValidLayer.Error.ToResponse() : secondValidLayer.ToResponse();
    }
    
    [HttpPut("{id:guid}/social-networks")]
    public async Task<ActionResult<Guid>> UpdateSocialNetworks(
        [FromServices] IValidator<UpdateSocialNetworksRequest> validator,
        [FromServices] UpdateSocialNetworksHandler handler,
        [FromRoute] Guid id,
        [FromBody] SocialNetworksDto dto,
        CancellationToken cancellationToken)
    {
        var request = new UpdateSocialNetworksRequest(id, dto);
        var firstValidLayer = await validator.ValidateAsync(request, cancellationToken);
        if (firstValidLayer.IsValid == false)
            return firstValidLayer.ToValidationErrorResponse();

        var secondValidLayer = await handler.HandleAsync(request, cancellationToken);
        return secondValidLayer.IsFailure ? secondValidLayer.Error.ToResponse() : secondValidLayer.ToResponse();
    }
    
    [HttpPut("{id:guid}/transfer-details")]
    public async Task<ActionResult<Guid>> UpdateTransferDetails(
        [FromServices] IValidator<UpdateTransferDetailsRequest> validator,
        [FromServices] UpdateTransferDetailsHandler handler,
        [FromRoute] Guid id,
        [FromBody] TransferDetailsDto dto,
        CancellationToken cancellationToken)
    {
        var request = new UpdateTransferDetailsRequest(id, dto);
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