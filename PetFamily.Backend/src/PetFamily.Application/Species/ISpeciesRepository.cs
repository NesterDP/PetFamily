using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;

namespace PetFamily.Application.Species;

public interface ISpeciesRepository
{
    Task<Guid> AddAsync(Domain.SpeciesContext.Entities.Species species, CancellationToken cancellationToken = default);

    Guid Save(Domain.SpeciesContext.Entities.Species species, CancellationToken cancellationToken = default);

    Guid Delete(Domain.SpeciesContext.Entities.Species species, CancellationToken cancellationToken = default);
    Task<Result<Domain.SpeciesContext.Entities.Species, Error>> GetByIdAsync(SpeciesId id, CancellationToken cancellationToken = default);
    
    Task<Result<Domain.SpeciesContext.Entities.Species, Error>> GetByNameAsync(string speciesName, CancellationToken cancellationToken = default);
}