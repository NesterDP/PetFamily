using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Controllers.Species.Requests;
using PetFamily.API.Extensions;
using PetFamily.Application.Species.Commands.DeleteBreedById;
using PetFamily.Application.Species.Commands.DeleteSpeciesById;
using PetFamily.Application.Species.Queries.GetBreedsBySpeciesId;
using PetFamily.Application.Species.Queries.GetSpeciesWithPagination;

namespace PetFamily.API.Controllers.Species;

public class SpeciesController : ApplicationController
{
    [HttpGet]
    public async Task<ActionResult> GetAllSpecies(
        [FromQuery] GetSpeciesWithPaginationRequest request,
        [FromServices] GetSpeciesWithPaginationHandler handler,
        CancellationToken cancellationToken)
    {
        var query = request.ToQuery();
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/breeds")]
    public async Task<ActionResult> GetAllBreeds(
        [FromRoute] Guid id,
        [FromServices] GetBreedsBySpeciesIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetBreedsBySpeciesIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteSpecies(
        [FromRoute] Guid id,
        [FromServices] DeleteSpeciesByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSpeciesByIdCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpDelete("{id:guid}/breed/{breedId:guid}")]
    public async Task<ActionResult> DeleteBreed(
        [FromRoute] Guid id,
        [FromRoute] Guid breedId,
        [FromServices] DeleteBreedByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBreedByIdCommand(id, breedId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
}