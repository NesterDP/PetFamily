using Microsoft.EntityFrameworkCore;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;

namespace PetFamily.Volunteers.Application.Queries.GetPetById;

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
            .FirstOrDefaultAsync(v => v.Id == query.Id && v.IsDeleted == false, cancellationToken);
        
        if (result != null)
            result.Photos = result.Photos.OrderByDescending(p => p.Main).ToArray();
        
        return result;
    }
}