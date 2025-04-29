using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Security.Authorization;
using PetFamily.Volunteers.Application.Commands.AddPet;
using PetFamily.Volunteers.Application.Commands.ChangePetPosition;
using PetFamily.Volunteers.Application.Commands.CompleteUploadPhotosToPet;
using PetFamily.Volunteers.Application.Commands.Delete;
using PetFamily.Volunteers.Application.Commands.DeletePet;
using PetFamily.Volunteers.Application.Commands.DeletePetPhotos;
using PetFamily.Volunteers.Application.Commands.StartUploadPhotosToPet;
using PetFamily.Volunteers.Application.Commands.UpdateMainInfo;
using PetFamily.Volunteers.Application.Commands.UpdatePetInfo;
using PetFamily.Volunteers.Application.Commands.UpdatePetMainPhoto;
using PetFamily.Volunteers.Application.Commands.UpdatePetStatus;
using PetFamily.Volunteers.Application.Queries.GetVolunteerById;
using PetFamily.Volunteers.Application.Queries.GetVolunteersWithPagination;
using PetFamily.Volunteers.Presentation.Volunteers.Requests;

namespace PetFamily.Volunteers.Presentation.Volunteers;

public class VolunteersController : ApplicationController
{
    private readonly UserScopedData _userData;

    public VolunteersController(UserScopedData userData)
    {
        _userData = userData;
    }

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
    public async Task<ActionResult<Guid>> GetById(
        [FromRoute] Guid id,
        [FromServices] GetVolunteerByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetVolunteerByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }

    [Permission("volunteers.AddPet")]
    [HttpPost("pet")]
    public async Task<ActionResult<Guid>> AddPet(
        [FromBody] AddPetRequest request,
        [FromServices] AddPetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.StartUploadPhotosToPet")]
    [HttpPost("pet/{petId:guid}/photos-start")]
    public async Task<ActionResult<Guid>> StartUploadPhotosToPet(
        [FromRoute] Guid petId,
        [FromBody] StartUploadPhotosToPetRequest request,
        [FromServices] StartUploadPhotosToPetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.CompleteUploadPhotosToPet")]
    [HttpPost("pet/{petId:guid}/photos-complete")]
    public async Task<ActionResult<Guid>> CompleteUploadPhotosToPet(
        [FromRoute] Guid petId,
        [FromBody] CompleteUploadPhotosToPetRequest request,
        [FromServices] CompleteUploadPhotosToPetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.UpdateMainInfo")]
    [HttpPut("main-info")]
    public async Task<ActionResult<Guid>> UpdateMainInfo(
        [FromServices] UpdateMainInfoHandler handler,
        [FromBody] UpdateMainInfoRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId);
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
    [HttpPut("pet/{petId:guid}/position")]
    public async Task<ActionResult<Guid>> ChangePetPosition(
        [FromRoute] Guid petId,
        [FromBody] ChangePetPositionRequest request,
        [FromServices] ChangePetPositionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.UpdatePetInfo")]
    [HttpPut("pet/{petId:guid}/info")]
    public async Task<ActionResult<Guid>> PetInfo(
        [FromRoute] Guid petId,
        [FromBody] UpdatePetInfoRequest request,
        [FromServices] UpdatePetInfoHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.UpdatePetStatus")]
    [HttpPut("pet/{petId:guid}/help-status")]
    public async Task<ActionResult<Guid>> PetHelpStatus(
        [FromRoute] Guid petId,
        [FromBody] UpdatePetHelpStatusRequest request,
        [FromServices] UpdatePetHelpStatusHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.UpdatePetMainPhoto")]
    [HttpPut("pet/{petId:guid}/main-photo")]
    public async Task<ActionResult<Guid>> MainPhoto(
        [FromRoute] Guid petId,
        [FromBody] UpdatePetMainPhotoRequest request,
        [FromServices] UpdatePetMainPhotoHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.Delete")]
    [HttpDelete("{id:guid}/hard")]
    public async Task<ActionResult<Guid>> DeleteVolunteerHard(
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
    public async Task<ActionResult<Guid>> DeleteVolunteerSoft(
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteVolunteerCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.DeletePetPhotos")]
    [HttpDelete("pet/{petId:guid}/photos")]
    public async Task<ActionResult<Guid>> DeletePetPhotos(
        [FromRoute] Guid petId,
        [FromBody] DeletePetPhotosRequest request,
        [FromServices] DeletePetPhotosHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.DeletePet")]
    [HttpDelete("pet/{petId:guid}/soft")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid petId,
        [FromServices] SoftDeletePetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeletePetCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.DeletePet")]
    [HttpDelete("pet/{petId:guid}/hard")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid petId,
        [FromServices] HardDeletePetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeletePetCommand(_userData.UserId, petId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
}