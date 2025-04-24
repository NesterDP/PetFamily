using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.SpeciesNS.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Species.Application.Commands.DeleteSpeciesById;
using PetFamily.Species.Domain.Entities;

namespace PetFamily.IntegrationTests.SpeciesNS.HandlerTests;

public class DeleteSpeciesByIdHandlerTests : SpeciesTestsBase
{
    private readonly ICommandHandler<Guid, DeleteSpeciesByIdCommand> _sut;

    public DeleteSpeciesByIdHandlerTests(SpeciesTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteSpeciesByIdCommand>>();
    }

    [Fact]
    public async Task DeleteSpeciesById_success_should_delete_species_from_database_and_all_its_breeds()
    {
        // arrange
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        species.AddBreed(new Breed(Guid.NewGuid(), Name.Create("test breed 1").Value));
        species.AddBreed(new Breed(Guid.NewGuid(), Name.Create("test breed 2").Value));
        await SpeciesWriteDbContext.SaveChangesAsync();
        var command = new DeleteSpeciesByIdCommand(species.Id);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var deletedSpecies = await SpeciesWriteDbContext.Species.FirstOrDefaultAsync(v => v.Id == species.Id);

        // species and all its breeds should be deleted from database
        deletedSpecies.Should().BeNull();

        foreach (var breed in species.Breeds)
        {
            var deletedBreed = await SpeciesReadDbContext.Breeds.FirstOrDefaultAsync(b => b.Id == breed.Id);
            deletedBreed.Should().BeNull();
        }
    }

    [Fact]
    public async Task DeleteSpeciesById_failure_should_not_delete_species_and_any_of_its_breeds_from_database()
    {
        // arrange
        int PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        species.AddBreed(new Breed(Guid.NewGuid(), Name.Create("test breed 1").Value));
        species.AddBreed(new Breed(Guid.NewGuid(), Name.Create("test breed 2").Value));
        await SpeciesWriteDbContext.SaveChangesAsync();
        
        var volunteer1 = await DataGenerator
            .SeedVolunteerWithPets(VolunteersWriteDbContext, PET_COUNT, species.Id, species.Breeds[0].Id);
        var volunteer2 = await DataGenerator
            .SeedVolunteerWithPets(VolunteersWriteDbContext, PET_COUNT, species.Id, species.Breeds[1].Id);
        
        var command = new DeleteSpeciesByIdCommand(species.Id);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        var remainedSpecies = await SpeciesWriteDbContext.Species.FirstOrDefaultAsync(v => v.Id == species.Id);

        // species and all its breeds are still in database
        remainedSpecies.Should().NotBeNull();

        foreach (var br in species.Breeds)
        {
            var remainedBreed = await SpeciesReadDbContext.Breeds.FirstOrDefaultAsync(b => b.Id == br.Id);
            remainedBreed.Should().NotBeNull();
        }
    }
}