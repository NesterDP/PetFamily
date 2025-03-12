using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Core.CustomErrors;
using PetFamily.Species.Application.SpeciesManagement;
using PetFamily.Species.Domain.Entities;
using PetFamily.Species.Domain.ValueObjects.SpeciesVO;
using PetFamily.Species.Infrastructure.DbContexts;

namespace PetFamily.Species.Infrastructure.Repositories;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly WriteDbContext _context;

    public SpeciesRepository(WriteDbContext dbContext)
    {
        _context = dbContext;
    }
    public async Task<Guid> AddAsync(Domain.Entities.Species species, CancellationToken cancellationToken = default)
    {
        await _context.Species.AddAsync(species, cancellationToken);
        return species.Id.Value;
    }
    
    public Guid Save(Domain.Entities.Species species, CancellationToken cancellationToken = default)
    {
        _context.Species.Attach(species);
        return species.Id.Value;
    }
    
    public Guid Delete(Domain.Entities.Species species, CancellationToken cancellationToken = default)
    {
        _context.Species.Remove(species);
        return species.Id.Value;
    }
    
    public async Task<Result<Domain.Entities.Species, Error>> GetByNameAsync(string speciesName,
        CancellationToken cancellationToken = default)
    {
        var species = await _context.Species
            .Include(v => v.Breeds)
            .FirstOrDefaultAsync(v => v.Name.Value == speciesName, cancellationToken);

        if (species == null)
            return Errors.General.ValueNotFound();

        return species;
    }
    
    public async Task<Result<Domain.Entities.Species, Error>> GetByIdAsync(SpeciesId id,
        CancellationToken cancellationToken = default)
    {
        var species = await _context.Species
            .Include(v => v.Breeds)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (species == null)
            return Errors.General.ValueNotFound(id.Value);

        return species;
    }
}