using PetFamily.Core.Abstractions;

// ReSharper disable NotAccessedPositionalProperty.Global
namespace PetFamily.Volunteers.Application.Queries.GetFilteredPetsWithPaginationDapper;

public record GetFilteredPetsWithPaginationDapperQuery(
    int Page,
    int PageSize,
    Guid? OwnerId = null,
    string? Name = null,
    Guid? SpeciesId = null,
    Guid? BreedId = null,
    string? Color = null,
    string? City = null,
    string? House = null,
    string? Apartment = null,
    float? Weight = null,
    float? Height = null,
    string? OwnerPhoneNumber = null,
    bool? IsCastrated = null,
    int? Age = null,
    bool? IsVaccinated = null,
    string? HelpStatus = null,
    string? SortBy = null,
    string? SortDirection = null) : IQuery;