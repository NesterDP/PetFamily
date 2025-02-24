using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.SpeciesRepositoryInterface;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.SpeciesContext.Entities;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;

namespace PetFamily.Infrastructure.Repositories;

public class SpeciesRepository(ApplicationDbContext context) : ISpeciesRepository
{
    public async Task<Guid> AddAsync(Species species, CancellationToken cancellationToken = default)
    {
        await context.Species.AddAsync(species, cancellationToken);
        
        //await context.SaveChangesAsync(cancellationToken);

        return species.Id.Value;
    }
    
    public async Task<Guid> SaveAsync(Species species, CancellationToken cancellationToken = default)
    {
        context.Attach(species);
        await context.SaveChangesAsync(cancellationToken);
        return species.Id.Value;
    }

    public async Task<Result<Species, Error>> GetByIdAsync(SpeciesId id,
        CancellationToken cancellationToken = default)
    {
        var species = await context.Species
            .Include(v => v.Breeds)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (species == null)
            return Errors.General.ValueNotFound();

        return species;
    }
    
    public async Task<Result<Species, Error>> GetByNameAsync(string speciesName,
        CancellationToken cancellationToken = default)
    {
        var species = await context.Species
            .Include(v => v.Breeds)
            .FirstOrDefaultAsync(v => v.Name.Value == speciesName, cancellationToken);

        if (species == null)
            return Errors.General.ValueNotFound();

        return species;
    }

    public async Task<Guid> DeleteAsync(Species species, CancellationToken cancellationToken = default)
    {
        context.Remove(species);
        await context.SaveChangesAsync();
        return species.Id.Value;
    }
}