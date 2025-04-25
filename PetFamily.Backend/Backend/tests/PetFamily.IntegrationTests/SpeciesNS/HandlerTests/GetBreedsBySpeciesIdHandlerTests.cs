using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Breed;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.SpeciesNS.Heritage;
using PetFamily.Species.Application.Queries.GetBreedsBySpeciesId;

namespace PetFamily.IntegrationTests.SpeciesNS.HandlerTests;

public class GetBreedsBySpeciesIdHandlerTests : SpeciesTestsBase
{
    private readonly IQueryHandler<List<BreedDto>, GetBreedsBySpeciesIdQuery> _sut;

    public GetBreedsBySpeciesIdHandlerTests(SpeciesTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<List<BreedDto>, GetBreedsBySpeciesIdQuery>>();
    }

    [Fact]
    public async Task GetBreedsBySpeciesId_returns_all_breeds_for_speciesId()
    {
        // arrange
        const int REQUIRED_BREED_COUNT = 3;
        var species1 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);

        var species2 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species2.Id);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species2.Id);
        var query = new GetBreedsBySpeciesIdQuery(species1.Id);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.Count.Should().Be(REQUIRED_BREED_COUNT);
    }

    [Fact]
    public async Task GetBreedsBySpeciesId_returns_empty_list_if_there_are_no_breeds_for_speciesId()
    {
        // arrange
        var species1 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);

        var species2 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var query = new GetBreedsBySpeciesIdQuery(species2.Id);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBreedsBySpeciesId_returns_empty_list_if_there_are_no_such_speciesId()
    {
        // arrange
        var species1 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);
        await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);

        var query = new GetBreedsBySpeciesIdQuery(Guid.NewGuid());

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Should().BeEmpty();
    }
}