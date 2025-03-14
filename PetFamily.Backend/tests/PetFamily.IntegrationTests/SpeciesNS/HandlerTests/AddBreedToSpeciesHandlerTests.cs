using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.SpeciesNS.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Species.Application.Commands.AddBreedToSpecies;

namespace PetFamily.IntegrationTests.SpeciesNS.HandlerTests;

public class AddBreedToSpeciesHandlerTests : SpeciesTestsBase
{
    private readonly ICommandHandler<Guid, AddBreedToSpeciesCommand> _sut;

    public AddBreedToSpeciesHandlerTests(SpeciesTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, AddBreedToSpeciesCommand>>();
    }

    [Fact]
    public async Task CreateSpecies_success_should_return_guid_of_created_species()
    {
        // arrange
        var BREED_NAME = "Test Breed";
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var existedBred = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var command = new AddBreedToSpeciesCommand(species.Id, BREED_NAME);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        var updatedSpecies = await SpeciesWriteDbContext.Species.FirstOrDefaultAsync(v => v.Id == species.Id);
        species.Breeds.Count.Should().Be(2); // both existed and added breeds are now in database
        species.Breeds.FirstOrDefault(breed => breed.Name.Value == BREED_NAME).Should().NotBeNull();
    }
}