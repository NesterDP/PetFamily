using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.Queries.GetVolunteerById;

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

        var result = await customQuery.FirstOrDefaultAsync();
        
        return result;
    }
}