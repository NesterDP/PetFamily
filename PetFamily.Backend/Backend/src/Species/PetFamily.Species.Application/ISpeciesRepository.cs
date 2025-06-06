using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Species.Application;

public interface ISpeciesRepository
{
    Task<Guid> AddAsync(Domain.Entities.Species species, CancellationToken cancellationToken = default);

    Guid Save(Domain.Entities.Species species, CancellationToken cancellationToken = default);

    Guid Delete(Domain.Entities.Species species, CancellationToken cancellationToken = default);

    Task<Result<Domain.Entities.Species, Error>> GetByIdAsync(
        SpeciesId id, CancellationToken cancellationToken = default);

    Task<Result<Domain.Entities.Species, Error>> GetByNameAsync(
        string speciesName, CancellationToken cancellationToken = default);
}