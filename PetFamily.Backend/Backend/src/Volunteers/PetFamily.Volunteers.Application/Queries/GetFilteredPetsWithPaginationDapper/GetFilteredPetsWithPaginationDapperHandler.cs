using System.Text;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Models;

namespace PetFamily.Volunteers.Application.Queries.GetFilteredPetsWithPaginationDapper;

public class GetFilteredPetsWithPaginationDapperHandler : IQueryHandler<
    PagedList<PetDto>,
    GetFilteredPetsWithPaginationDapperQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<GetFilteredPetsWithPaginationDapperHandler> _logger;

    public GetFilteredPetsWithPaginationDapperHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        ILogger<GetFilteredPetsWithPaginationDapperHandler> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
    }

    public async Task<PagedList<PetDto>> HandleAsync(
        GetFilteredPetsWithPaginationDapperQuery query,
        CancellationToken cancellationToken)
    {
        var connection = _sqlConnectionFactory.Create();

        var parameters = new DynamicParameters();

        var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM pets");

        var sql = new StringBuilder();

        var selector = new Selector();

        sql.Append("""
                   SELECT *
                   FROM pets
                   """);


        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            sql.Append(selector.Select("name = @Name"));
            parameters.Add("@Name", query.Name);
        }

        if (query.OwnerId != null)
        {
            sql.Append(selector.Select("volunteer_id = @VolunteerId"));
            parameters.Add("@VolunteerId", query.OwnerId);
        }

        sql.ApplySorting(parameters, query.SortBy, query.SortDirection);
        sql.ApplyPagination(parameters, query.Page, query.PageSize);
        //var test = sql.ToString();

        var result = await connection.QueryAsync<dynamic>(sql.ToString(), parameters);

        var pets = result.Select(row =>
        {
            var pet = new PetDto
            {
                Id = row.id,
                Name = row.name,
                OwnerId = row.volunteer_id,
                Description = row.description,
                SpeciesId = row.species_id,
                BreedId = row.breed_id,
                Color = row.color,
                HealthInfo = row.health_info,
                City = row.city,
                House = row.house,
                Apartment = row.apartment,
                Weight = row.weight_info,
                Height = row.height_info,
                OwnerPhoneNumber = row.owner_phone.ToString(),
                IsCastrated = row.is_castrated,
                DateOfBirth = row.date_of_birth,
                IsVaccinated = row.is_vaccinated,
                HelpStatus = row.help_status.ToString(),
                CreationDate = row.creation_date,
                Position = row.position,
                TransferDetails = JsonSerializer.Deserialize<TransferDetailDto[]>(row.transfer_details),
                Photos = JsonSerializer.Deserialize<PhotoDto[]>(row.photos)
            };
            return pet;
        }).ToList();

        foreach (var pet in pets)
        {
            pet.Photos = pet.Photos.OrderByDescending(p => p.Main).ToArray();
        }

        return new PagedList<PetDto>()
        {
            Items = pets.ToList(),
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Page = query.PageSize
        };
    }
}