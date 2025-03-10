using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Controllers.Volunteers.Requests;
using PetFamily.API.Extensions;
using PetFamily.API.Processors;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Models;
using PetFamily.Application.Volunteers.Commands.AddPet;
using PetFamily.Application.Volunteers.Commands.ChangePetPosition;
using PetFamily.Application.Volunteers.Commands.Create;
using PetFamily.Application.Volunteers.Commands.Delete;
using PetFamily.Application.Volunteers.Commands.DeletePet;
using PetFamily.Application.Volunteers.Commands.DeletePetPhotos;
using PetFamily.Application.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Application.Volunteers.Commands.UpdatePetInfo;
using PetFamily.Application.Volunteers.Commands.UpdatePetMainPhoto;
using PetFamily.Application.Volunteers.Commands.UpdatePetStatus;
using PetFamily.Application.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.Commands.UpdateTransferDetails;
using PetFamily.Application.Volunteers.Commands.UploadPhotosToPet;
using PetFamily.Application.Volunteers.Queries.GetVolunteerById;
using PetFamily.Application.Volunteers.Queries.GetVolunteersWithPagination;


namespace PetFamily.API.Controllers.Volunteers;


public class VolunteersController : ApplicationController
{
    [HttpGet]
    public async Task<ActionResult> Get(
        [FromQuery] GetVolunteersWithPaginationRequest request,
        [FromServices] GetVolunteersWithPaginationHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Guid>>  Get(
        [FromRoute] Guid id,
        [FromServices] GetVolunteerByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetVolunteerByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [HttpPost("{id:guid}/pet")]
    public async Task<ActionResult<Guid>> AddPet(
        [FromRoute] Guid id,
        [FromBody] AddPetRequest request,
        [FromServices] AddPetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPost("{id:guid}/pet/{petId:guid}/photos")]
    public async Task<ActionResult<Guid>> UploadPhotosToPet(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromForm] UploadPhotosToPetRequest request,
        [FromServices] UploadPhotosToPetHandler handler,
        CancellationToken cancellationToken)
    {
        await using var fileProcessor = new FormFileProcessor();
        var fileDtos = fileProcessor.Process(request.Files);

        var command = new UploadPhotosToPetCommand(id, petId, fileDtos);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPut("{id:guid}/main-info")]
    public async Task<ActionResult<Guid>> UpdateMainInfo(
        [FromServices] UpdateMainInfoHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPut("{id:guid}/social-networks")]
    public async Task<ActionResult<Guid>> UpdateSocialNetworks(
        [FromServices] UpdateSocialNetworksHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateSocialNetworksRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPut("{id:guid}/transfer-details")]
    public async Task<ActionResult<Guid>> UpdateTransferDetails(
        [FromServices] UpdateTransferDetailsHandler handler,
        [FromRoute] Guid id,
        [FromBody] UpdateTransferDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [HttpPut("{id:guid}/pet/{petId:guid}/position")]
    public async Task<ActionResult<Guid>> ChangePetPosition(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromBody] ChangePetPositionRequest request,
        [FromServices] ChangePetPositionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [HttpPut("{id:guid}/pet/{petId:guid}/info")]
    public async Task<ActionResult<Guid>> PetInfo(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromBody] UpdatePetInfoRequest request,
        [FromServices] UpdatePetInfoHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [HttpPut("{id:guid}/pet/{petId:guid}/help-status")]
    public async Task<ActionResult<Guid>> PetHelpStatus(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromBody] UpdatePetHelpStatusRequest request,
        [FromServices] UpdatePetHelpStatusHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [HttpPut("{id:guid}/pet/{petId:guid}/main-photo")]
    public async Task<ActionResult<Guid>> MainPhoto(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromBody] UpdatePetMainPhotoRequest request,
        [FromServices] UpdatePetMainPhotoHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpDelete("{id:guid}/hard")]
    public async Task<ActionResult<Guid>> Delete(
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
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteVolunteerCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [HttpDelete("{id:guid}/pet/{petId:guid}/photos")]
    public async Task<ActionResult<Guid>> DeletePetPhotos(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromBody] DeletePetPhotosRequest request,
        [FromServices] DeletePetPhotosHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [HttpDelete("{id:guid}/pet/{petId:guid}/soft")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromServices] SoftDeletePetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeletePetCommand(id, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [HttpDelete("{id:guid}/pet/{petId:guid}/hard")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromServices] HardDeletePetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeletePetCommand(id, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
}