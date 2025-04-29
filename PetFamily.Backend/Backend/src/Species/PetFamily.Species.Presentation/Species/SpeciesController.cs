using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Security.Authorization;
using PetFamily.Species.Application.Commands.AddBreedToSpecies;
using PetFamily.Species.Application.Commands.Create;
using PetFamily.Species.Application.Commands.DeleteBreedById;
using PetFamily.Species.Application.Commands.DeleteSpeciesById;
using PetFamily.Species.Application.Queries.GetSpeciesWithPagination;
using PetFamily.Species.Presentation.Species.Requests;

namespace PetFamily.Species.Presentation.Species;

public class SpeciesController : ApplicationController
{
    [Permission("species.GetSpeciesWithPagination")]
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

    [Permission("species.Create")]
    [HttpPost]
    public async Task<ActionResult> CreateSpecies(
        [FromBody] CreateSpeciesRequest request,
        [FromServices] CreateSpeciesHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("species.AddBreedToSpecies")]
    [HttpPost("{id:guid}/breed")]
    public async Task<ActionResult> AddBreedToSpecies(
        [FromRoute] Guid id,
        [FromBody] AddBreedToSpeciesRequest request,
        [FromServices] AddBreedToSpeciesHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [Permission("species.DeleteSpeciesById")]
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

    [Permission("species.DeleteBreedById")]
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