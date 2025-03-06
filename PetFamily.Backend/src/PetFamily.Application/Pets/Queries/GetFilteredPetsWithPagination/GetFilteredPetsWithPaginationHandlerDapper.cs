using System.Text;
using System.Text.Json;
using Dapper;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Models;

namespace PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;

public class GetFilteredPetsWithPaginationHandlerDapper : IQueryHandler<
    PagedList<PetDto>,
    GetFilteredPetsWithPaginationQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetFilteredPetsWithPaginationHandlerDapper(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<PagedList<PetDto>> HandleAsync(
        GetFilteredPetsWithPaginationQuery query,
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
        var test = sql.ToString();

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

        return new PagedList<PetDto>()
        {
            Items = pets.ToList(),
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Page = query.PageSize
        };
    }
}

public class Selector
{
    private const string WHERE = "\nWHERE ";
    private const string AND = "\nAND ";
    private int counter = 0;

    public string Select(string filter)
    {
        var head = " " + (counter == 0 ? WHERE : AND) +"(";
        var body = filter + ")";
        counter++;
        return head + body;
    }
}

public static class DapperExtensions
{
    public static void ApplySorting(
        this StringBuilder sqlBuilder,
        DynamicParameters parameters,
        string? sortBy,
        string? sortDirection)
    {
        parameters.Add("@SortBy", sortBy?.ToLower() ?? "id");
        parameters.Add("@SortDirection", sortDirection ?? "desc");

        sqlBuilder.Append($"\nORDER BY @SortBy, @SortDirection");
    }

    public static void ApplyPagination(
        this StringBuilder sqlBuilder,
        DynamicParameters parameters,
        int page,
        int pageSize)
    {
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@Offset", (page - 1) * pageSize);

        sqlBuilder.Append("\nLimit @PageSize OFFSET @Offset");
    }
}