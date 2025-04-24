using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.Volunteers.Application.Queries.CheckSpeciesToPetExistence;
using PetFamily.Volunteers.Contracts;
using PetFamily.Volunteers.Contracts.Requests;

namespace PetFamily.Volunteers.Presentation.Contracts;

public class SpeciesToPetExistenceContract : ISpeciesToPetExistenceContract
{
    private readonly CheckSpeciesToPetExistenceHandler _handler;

    public SpeciesToPetExistenceContract(CheckSpeciesToPetExistenceHandler handler)
    {
        _handler = handler;
    }

    public async Task<UnitResult<Error>> SpeciesToPetExistence(
        SpeciesToPetExistenceRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new CheckSpeciesToPetExistenceQuery(request.SpeciesId);
        return await _handler.HandleAsync(query, cancellationToken);
    }
}