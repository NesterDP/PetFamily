using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;

public record GetFilteredPetsWithPaginationQuery(
    int Page,
    int PageSize,
    Guid? OwnerId,
    string? Name,
    Guid? SpeciesId,
    Guid? BreedId,
    string? Color,
    string? City,
    string? House,
    string? Apartment,
    float? Weight,
    float? Height,
    string? OwnerPhoneNumber,
    bool? IsCastrated,
    int? Age,
    bool? IsVaccinated,
    string? HelpStatus,
    string? SortBy,
    string? SortDirection) : IQuery;