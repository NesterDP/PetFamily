using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Pet;

namespace PetFamily.Application.Pets.Queries.GetPetById;

public class GetPetByIdHandler : IQueryHandler<PetDto, GetPetByIdQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetPetByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<PetDto> HandleAsync(
        GetPetByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _readDbContext.Pets
            .FirstOrDefaultAsync(v => v.Id == query.Id, cancellationToken);
        
        return result;
    }
}