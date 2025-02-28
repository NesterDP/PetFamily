using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.SpeciesContext.Entities;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;

namespace PetFamily.Application.SpeciesRepositoryInterface;

public interface ISpeciesRepository
{
    Task<Guid> AddAsync(Species species, CancellationToken cancellationToken = default);

    Guid Save(Species species, CancellationToken cancellationToken = default);

    Guid Delete(Species species, CancellationToken cancellationToken = default);
    Task<Result<Species, Error>> GetByIdAsync(SpeciesId id, CancellationToken cancellationToken = default);
    
    Task<Result<Species, Error>> GetByNameAsync(string speciesName, CancellationToken cancellationToken = default);
}