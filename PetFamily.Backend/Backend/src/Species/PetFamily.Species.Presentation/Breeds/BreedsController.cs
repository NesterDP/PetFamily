using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Security.Authorization;
using PetFamily.Species.Application.Queries.GetBreedsBySpeciesId;

namespace PetFamily.Species.Presentation.Breeds;

public class BreedsController : ApplicationController
{
    [Permission("species.GetBreedsBySpeciesId")]
    [HttpGet("/species/{id:guid}")]
    public async Task<ActionResult> AllBreedsBySpeciesId(
        [FromRoute] Guid id,
        [FromServices] GetBreedsBySpeciesIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetBreedsBySpeciesIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
}