using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.Application.Pets.Queries;

namespace PetFamily.API.Controllers.Pets;

[ApiController]
[Route("[controller]")]
public class PetsController : ApplicationController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Guid>>  Get(
        [FromRoute] Guid id,
        [FromServices] GetPetByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPetByIdQuery(id);
        var result = await handler.HandlerAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
}