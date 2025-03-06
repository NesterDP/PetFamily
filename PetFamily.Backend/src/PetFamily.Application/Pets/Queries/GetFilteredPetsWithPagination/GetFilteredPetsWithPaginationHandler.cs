using System.Linq.Expressions;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Extensions;
using PetFamily.Application.Models;
using PetFamily.Application.Volunteers.Queries.GetVolunteersWithPagination;

namespace PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;



public class GetFilteredPetsWithPaginationHandler : IQueryHandler<
    PagedList<PetDto>,
    GetFilteredPetsWithPaginationQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetFilteredPetsWithPaginationHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<PagedList<PetDto>> HandleAsync(
        GetFilteredPetsWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var petsQuery = _readDbContext.Pets;
        
        petsQuery = petsQuery.WhereIf(
            query.OwnerId != null,
            p => p.OwnerId == query.OwnerId);
        
        petsQuery = petsQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.Name),
            p => p.Name.Contains(query.Name!));
        
        petsQuery = petsQuery.WhereIf(
            query.Age != null,
            p => (DateTime.UtcNow - p.DateOfBirth).Days / 365 < query.Age);
        
        petsQuery = petsQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.Color),
            p => p.Name.Contains(query.Color!));
        
        petsQuery = petsQuery.WhereIf(
            query.SpeciesId != null,
            p => p.SpeciesId == query.SpeciesId);
        
        petsQuery = petsQuery.WhereIf(
            query.BreedId != null,
            p => p.BreedId == query.BreedId);
        
        petsQuery = petsQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.City),
            p => p.City == query.City!);
        
        petsQuery = petsQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.House),
            p => p.House == query.House!);
        
        petsQuery = petsQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.Apartment),
            p => p.Apartment == query.Apartment!);
        
        petsQuery = petsQuery.WhereIf(
            query.IsVaccinated != null,
            p => p.IsVaccinated == query.IsVaccinated!);
        
        petsQuery = petsQuery.WhereIf(
            query.IsCastrated != null,
            p => p.IsCastrated == query.IsCastrated!);
        
        petsQuery = petsQuery.WhereIf(
            query.OwnerPhoneNumber != null,
            p => p.OwnerPhoneNumber == query.OwnerPhoneNumber!);
        
        petsQuery = petsQuery.WhereIf(
            query.Weight != null,
            p => (p.Weight < query.Weight!));
        
        petsQuery = petsQuery.WhereIf(
            query.Height != null,
            p => (p.Height < query.Height!));
        
        petsQuery = petsQuery.WhereIf(
            query.HelpStatus != null,
            p => (p.HelpStatus == query.HelpStatus!));
        
        
        Expression<Func<PetDto, object>> keySelector = query.SortBy?.ToLower() switch
        {
            "name" => (dto) => dto.Name,
            "ownerId" => (dto) => dto.OwnerId,
            "age" => (dto) => (DateTime.UtcNow - dto.DateOfBirth).Days,
            "city" => (dto) => dto.City,
            "house" => (dto) => dto.House,
            "apartment" => (dto) => dto.Apartment == null ? dto.Id : dto.Apartment,
            "breed" => (dto) => dto.BreedId,
            "species" => (dto) => dto.SpeciesId,
            "color" => (dto) => dto.Color,
            "weight" => (dto) => dto.Weight,
            "height" => (dto) => dto.Height,
            _ => (dto) => dto.Id
        };

        petsQuery = query.SortDirection?.ToLower() == "desc"
            ? petsQuery.OrderByDescending(keySelector)
            : petsQuery.OrderBy(keySelector);
        
        
        var result = await petsQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);
        
        foreach (var pet in result.Items)
        {
            pet.Photos = pet.Photos.OrderByDescending(photo => photo.Main).ToArray();
        }

        return result;
    }
}