using CSharpFunctionalExtensions;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.GetVolunteer;

public class GetVolunteerHandler
{
    private readonly IVolunteersRepository _volunteersRepository;

    public GetVolunteerHandler(IVolunteersRepository volunteersRepository)
    {
        _volunteersRepository = volunteersRepository;
    }

    public async Task<Result<Volunteer, Error>> Handle(VolunteerId id, CancellationToken cancellationToken = default)
    {
        var result = await _volunteersRepository.GetByIdAsync(id, cancellationToken);

        return result;
    }
}