using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.SpeciesNS.Heritage;
using PetFamily.Species.Application.Queries.GetSpeciesWithPagination;

namespace PetFamily.IntegrationTests.SpeciesNS.HandlerTests;


public class GetSpeciesWithPaginationHandlerTests : SpeciesTestsBase
{
    private readonly GetSpeciesWithPaginationHandler _sut;

    public GetSpeciesWithPaginationHandlerTests(SpeciesTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<GetSpeciesWithPaginationHandler>();
    }
    
    [Fact]
    public async Task GetSpeciesWithPagination_returns_all_species()
    {
        // arrange
        var SPECIES_COUNT = 3;
        var PAGE = 1;
        var PAGE_SIZE = 10;
        var species1 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var species2 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var species3 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);

        var query = new GetSpeciesWithPaginationQuery(PAGE, PAGE_SIZE);
        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.TotalCount.Should().Be(SPECIES_COUNT);
    }
    
    [Fact]
    public async Task GetSpeciesWithPagination_returns_empty_items_list_if_there_are_no_species()
    {
        // arrange
        var SPECIES_COUNT = 3;
        var PAGE = 1;
        var PAGE_SIZE = 10;
        
        var query = new GetSpeciesWithPaginationQuery(PAGE, PAGE_SIZE);
        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.Items.Should().BeEmpty();
    }
}