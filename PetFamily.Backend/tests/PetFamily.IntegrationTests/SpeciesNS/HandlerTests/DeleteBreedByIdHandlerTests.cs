using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Species.Commands.DeleteBreedById;
using PetFamily.Application.Species.Commands.DeleteSpeciesById;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.SpeciesNS.Heritage;
using PetFamily.Core.Abstractions;

namespace PetFamily.IntegrationTests.SpeciesNS.HandlerTests;

public class DeleteBreedByIdHandlerTests : SpeciesTestsBase
{
    private readonly ICommandHandler<Guid, DeleteBreedByIdCommand> _sut;

    public DeleteBreedByIdHandlerTests(SpeciesTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteBreedByIdCommand>>();
    }

    [Fact]
    public async Task DeleteBreedById_success_should_delete_breed_from_database()
    {
        // arrange
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed1 = await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var breed2 = await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var command = new DeleteBreedByIdCommand(species.Id, breed1.Id);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var updatedSpecies = await WriteDbContext.Species.FirstOrDefaultAsync(s => s.Id == species.Id);
        updatedSpecies!.Breeds.Count.Should().Be(1);
        
        // selected breed was deleted
        updatedSpecies.Breeds.FirstOrDefault(breed => breed.Id == breed1.Id).Should().BeNull();
        
        // not selected breed of the same species is still in database
        updatedSpecies.Breeds.FirstOrDefault(breed => breed.Id == breed2.Id).Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteBreedById_failure_should_not_delete_breed_from_database_because_of_pet_with_such_breed()
    {
        // arrange
        var PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed = await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var command = new DeleteBreedByIdCommand(species.Id, breed.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, species.Id, breed.Id);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        var updatedSpecies = await WriteDbContext.Species.FirstOrDefaultAsync(s => s.Id == species.Id);
        updatedSpecies!.Breeds.Count.Should().Be(1);
        
        // selected breed should still be in database
        updatedSpecies.Breeds.FirstOrDefault(b => b.Id == breed.Id).Should().NotBeNull();
    }
}