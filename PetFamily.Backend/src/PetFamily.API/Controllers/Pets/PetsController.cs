using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Controllers.Pets.Requests;
using PetFamily.API.Extensions;
using PetFamily.Application.Pets.Queries;
using PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;
using PetFamily.Application.Pets.Queries.GetFilteredPetsWithPaginationDapper;
using PetFamily.Application.Pets.Queries.GetPetById;

namespace PetFamily.API.Controllers.Pets;

public class PetsController : ApplicationController
{
    [HttpGet]
    public async Task<ActionResult> GetAllPets(
        [FromQuery] GetFilteredPetsWithPaginationRequest request,
        [FromServices] GetFilteredPetsWithPaginationHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(
        [FromRoute] Guid id,
        [FromServices] GetPetByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPetByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
    
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