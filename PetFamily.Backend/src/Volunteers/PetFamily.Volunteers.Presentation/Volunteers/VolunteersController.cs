using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Authorization;
using PetFamily.Framework.Processors;
using PetFamily.Volunteers.Application.Commands.AddPet;
using PetFamily.Volunteers.Application.Commands.ChangePetPosition;
using PetFamily.Volunteers.Application.Commands.Create;
using PetFamily.Volunteers.Application.Commands.Delete;
using PetFamily.Volunteers.Application.Commands.DeletePet;
using PetFamily.Volunteers.Application.Commands.DeletePetPhotos;
using PetFamily.Volunteers.Application.Commands.UpdateMainInfo;
using PetFamily.Volunteers.Application.Commands.UpdatePetInfo;
using PetFamily.Volunteers.Application.Commands.UpdatePetMainPhoto;
using PetFamily.Volunteers.Application.Commands.UpdatePetStatus;
using PetFamily.Volunteers.Application.Commands.UpdateSocialNetworks;
using PetFamily.Volunteers.Application.Commands.UpdateTransferDetails;
using PetFamily.Volunteers.Application.Commands.UploadPhotosToPet;
using PetFamily.Volunteers.Application.Queries.GetVolunteerById;
using PetFamily.Volunteers.Application.Queries.GetVolunteersWithPagination;
using PetFamily.Volunteers.Presentation.Volunteers.Requests;

namespace PetFamily.Volunteers.Presentation.Volunteers;


public class VolunteersController : ApplicationController
{
    [Permission("volunteers.GetVolunteersWithPagination")]
    [HttpGet]
    public async Task<ActionResult> GetVolunteers(
        [FromQuery] GetVolunteersWithPaginationRequest request,
        [FromServices] GetVolunteersWithPaginationHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
    
    [Permission("volunteers.GetVolunteerById")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Guid>>  GetById(
        [FromRoute] Guid id,
        [FromServices] GetVolunteerByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetVolunteerByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
    
    [Permission("volunteers.Create")]
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
    
    [Permission("volunteers.AddPet")]
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

    [Permission("volunteers.UploadPhotosToPet")]
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

    [Permission("volunteers.UpdateMainInfo")]
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

    /*[Permission("")]
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
    }*/

    /*[Permission("volunteers.UpdateTransferDetails")]
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
    }*/
    
    [Permission("volunteers.ChangePetPosition")]
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
    
    [Permission("volunteers.UpdatePetInfo")]
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
    
    [Permission("volunteers.UpdatePetStatus")]
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
    
    [Permission("volunteers.UpdatePetMainPhoto")]
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

    [Permission("volunteers.Delete")]
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
    
    [Permission("volunteers.Delete")]
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
    
    [Permission( "volunteers.DeletePetPhotos")]
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
    
    [Permission("volunteers.DeletePet")]
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
    
    [Permission("volunteers.DeletePet")]
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