using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteerRepository : IVolunteersRepository
{
    private readonly WriteDbContext _context;

    public VolunteerRepository(WriteDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken = default)
    {
        await _context.Volunteers.AddAsync(volunteer, cancellationToken);
        return volunteer.Id.Value;
    }

    public Guid Save(Volunteer volunteer, CancellationToken cancellationToken = default)
    {
        _context.Volunteers.Attach(volunteer);
        return volunteer.Id.Value;
    }


    public Guid Delete(Volunteer volunteer, CancellationToken cancellationToken = default)
    {
        _context.Volunteers.Remove(volunteer);
        return volunteer.Id.Value;
    }

    public async Task<Result<Volunteer, Error>> GetByIdAsync(VolunteerId id,
        CancellationToken cancellationToken = default)
    {
        var volunteer = await _context.Volunteers
            .Include(v => v.AllOwnedPets)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (volunteer == null)
            return Errors.General.ValueNotFound();

        return volunteer;
    }
}