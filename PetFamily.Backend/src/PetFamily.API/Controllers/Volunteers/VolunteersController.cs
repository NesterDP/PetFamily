using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Controllers.Volunteers.Requests;
using PetFamily.API.Extensions;
using PetFamily.API.Processors;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.Create;
using PetFamily.Application.Volunteers.Delete;
using PetFamily.Application.Volunteers.DeletePetPhotos;
using PetFamily.Application.Volunteers.UpdateMainInfo;
using PetFamily.Application.Volunteers.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.UpdateTransferDetails;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;

namespace PetFamily.API.Controllers.Volunteers;

[ApiController]
[Route("[controller]")]
public class VolunteersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices] IValidator<CreateVolunteerCommand> validator,
        [FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateVolunteerCommand(
            request.VolunteerDto,
            request.SocialNetworksDto,
            request.TransferDetailsDto);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPut("{id:guid}/main-info")]
    public async Task<ActionResult<Guid>> UpdateMainInfo(
        [FromServices] IValidator<UpdateMainInfoCommand> validator,
        [FromServices] UpdateMainInfoHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMainInfoCommand(
            id,
            request.FullNameDto,
            request.Email,
            request.Description,
            request.Experience,
            request.PhoneNumber);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPut("{id:guid}/social-networks")]
    public async Task<ActionResult<Guid>> UpdateSocialNetworks(
        [FromServices] IValidator<UpdateSocialNetworksCommand> validator,
        [FromServices] UpdateSocialNetworksHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateSocialNetworksRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSocialNetworksCommand(
            id,
            request.SocialNetworksDto);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPut("{id:guid}/transfer-details")]
    public async Task<ActionResult<Guid>> UpdateTransferDetails(
        [FromServices] IValidator<UpdateTransferDetailsCommand> validator,
        [FromServices] UpdateTransferDetailsHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateTransferDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTransferDetailsCommand(
            id,
            request.TransferDetailsDto);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpDelete("{id:guid}/hard")]
    public async Task<ActionResult<Guid>> Delete(
        [FromServices] IValidator<DeleteVolunteerCommand> validator,
        [FromServices] HardDeleteVolunteerHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteVolunteerCommand(id);


        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpDelete("{id:guid}/soft")]
    public async Task<ActionResult<Guid>> Delete(
        [FromServices] IValidator<DeleteVolunteerCommand> validator,
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteVolunteerCommand(id);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }


    [HttpPost("{id:guid}/pet")]
    public async Task<ActionResult<Guid>> AddPet(
        [FromRoute] Guid id,
        [FromForm] AddPetRequest request,
        [FromServices] AddPetHandler handler,
        CancellationToken cancellationToken)
    {
        await using var fileProcessor = new FormFileProcessor();
        var fileDtos = fileProcessor.Process(request.Files);

        var command = new AddPetCommand(
            id,
            request.Name,
            request.Description,
            request.PetClassificationDto,
            request.Color,
            request.HealthInfo,
            request.AddressDto,
            request.Weight,
            request.Height,
            request.OwnerPhoneNumber,
            request.IsCastrated,
            request.DateOfBirth,
            request.IsVaccinated,
            request.HelpStatus,
            request.TransferDetailsDto,
            fileDtos);

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.ToResponse();
    }

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}")]
    public async Task<ActionResult<Guid>> DeletePetPhotos(
        [FromRoute] Guid volunteerId, Guid petId,
        [FromForm] DeletePetPhotosRequest request,
        [FromServices] DeletePetPhotosHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeletePetPhotosCommand(
            volunteerId,
            petId,
            request.PhotosNames.ToList());

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.ToResponse();
    }
}