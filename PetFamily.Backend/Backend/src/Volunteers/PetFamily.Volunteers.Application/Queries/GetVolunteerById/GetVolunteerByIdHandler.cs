using Microsoft.EntityFrameworkCore;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Volunteers.Application.Queries.GetVolunteerById;

public class GetVolunteerByIdHandler : IQueryHandler<VolunteerDto, GetVolunteerByIdQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetVolunteerByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<VolunteerDto> HandleAsync(
        GetVolunteerByIdQuery query,
        CancellationToken cancellationToken)
    {
        var customQuery = _readDbContext.Volunteers;

        customQuery = customQuery.Where(v => v.Id == query.Id && v.IsDeleted == false);
            //.Include(v => v.Pets);
        
        var result = await customQuery.FirstOrDefaultAsync();
        
        return result;
    }
}