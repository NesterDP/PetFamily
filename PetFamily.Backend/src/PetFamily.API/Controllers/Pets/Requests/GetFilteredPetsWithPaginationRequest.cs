using PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;

namespace PetFamily.API.Controllers.Pets.Requests;

public record GetFilteredPetsWithPaginationRequest(
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
    string? SortDirection)
{
    public GetFilteredPetsWithPaginationQuery ToQuery() =>  new GetFilteredPetsWithPaginationQuery(
        Page,
        PageSize,
        OwnerId,
        Name,
        SpeciesId,
        BreedId,
        Color,
        City,
        House,
        Apartment,
        Weight,
        Height,
        OwnerPhoneNumber,
        IsCastrated,
        Age,
        IsVaccinated,
        HelpStatus,
        SortBy,
        SortDirection);
}