using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Infrastructure.DbContexts;

namespace PetFamily.VolunteerRequests.Infrastructure.Repositories;

public class VolunteerRequestsRepository : IVolunteerRequestsRepository
{
    private readonly WriteDbContext _context;

    public VolunteerRequestsRepository(WriteDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Guid> AddAsync(VolunteerRequest volunteerRequest, CancellationToken cancellationToken = default)
    {
        await _context.VolunteerRequests.AddAsync(volunteerRequest, cancellationToken);
        return volunteerRequest.Id.Value;
    }

    public Guid Save(VolunteerRequest volunteerRequest, CancellationToken cancellationToken = default)
    {
        _context.VolunteerRequests.Attach(volunteerRequest);
        return volunteerRequest.Id.Value;
    }


    public Guid Delete(VolunteerRequest volunteerRequest, CancellationToken cancellationToken = default)
    {
        _context.VolunteerRequests.Remove(volunteerRequest);
        return volunteerRequest.Id.Value;
    }

    public async Task<Result<VolunteerRequest, Error>> GetByIdAsync(
        VolunteerRequestId id,
        CancellationToken cancellationToken = default)
    {
        var volunteerRequest = await _context.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (volunteerRequest == null)
            return Errors.General.ValueNotFound();

        return volunteerRequest;
    }

    public async Task<List<VolunteerRequest>> GetRequestsByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        var volunteerRequest = await _context.VolunteerRequests
            .Where(v => v.UserId == userId).ToListAsync(cancellationToken);

        return volunteerRequest;
    }
}