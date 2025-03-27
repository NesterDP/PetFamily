using CSharpFunctionalExtensions;
using PetFamily.Discussions.Application.Abstractions;
using PetFamily.Discussions.Infrastructure.DbContexts;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Discussions.Infrastructure.Repositories;

public class DiscussionsRepository : IDiscussionsRepository
{
    private readonly WriteDbContext _context;

    public DiscussionsRepository(WriteDbContext dbContext)
    {
        _context = dbContext;
    }

    /*public async Task<Guid> AddAsync(VolunteerRequest volunteerRequest, CancellationToken cancellationToken = default)
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

    public async Task<Result<VolunteerRequest, Error>> GetByIdAsync(VolunteerRequestId id,
        CancellationToken cancellationToken = default)
    {
        var volunteerRequest = await _context.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (volunteerRequest == null)
            return Errors.General.ValueNotFound();

        return volunteerRequest;
    }*/
}