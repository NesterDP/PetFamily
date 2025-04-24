using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Authorization;
using PetFamily.Volunteers.Application.Queries.GetFilteredPetsWithPagination;
using PetFamily.Volunteers.Application.Queries.GetFilteredPetsWithPaginationDapper;
using PetFamily.Volunteers.Application.Queries.GetPetById;
using PetFamily.Volunteers.Presentation.Pets.Requests;

namespace PetFamily.Volunteers.Presentation.Pets;

public class PetsController : ApplicationController
{
    [Permission("volunteers.GetFilteredPetsWithPagination")]
    [HttpGet]
    public async Task<ActionResult> GetAllPets(
        [FromQuery] GetFilteredPetsWithPaginationRequest request,
        [FromServices] GetFilteredPetsWithPaginationHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.GetPetById")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(
        [FromRoute] Guid id,
        [FromServices] GetPetByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPetByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("volunteers.GetFilteredPetsWithPaginationDapper")]
    [HttpGet("/dapper")]
    public async Task<ActionResult> GetAllPetsDapper(
        [FromQuery] GetFilteredPetsWithPaginationRequest request,
        [FromServices] GetFilteredPetsWithPaginationDapperHandler dapperHandler,
        CancellationToken cancellationToken)
    {
        var query = request.ToDapperQuery();
        var result = await dapperHandler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
}