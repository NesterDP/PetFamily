using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Volunteers.Commands.AddPet;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class AddPetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, AddPetCommand> _sut;

    public AddPetHandlerTests(VolunteersTestsWebFactory factory) : base(factory)
    { 
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, AddPetCommand>>();
    }
    
     [Fact]
    public async Task Add_pet_to_volunteer()
    {
        //base.SetupSuccessFileServiceMock();
        
        // arrange
        var volunteerId = await SeedVolunteer();
        var speciesId = await SeedSpecies();
        var breedId = await SeedBreed(speciesId);
        var command = Fixture.AddPetCommand(volunteerId);
        command.PetClassificationDto.SpeciesId = speciesId;
        command.PetClassificationDto.BreedId = breedId;
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        var voluntees = await WriteDbContext.Volunteers.ToListAsync();

        volunteer!.AllOwnedPets.Should().NotBeEmpty();
        volunteer!.AllOwnedPets.First().Id.Should().Be(result.Value);
        voluntees.Count().Should().Be(1);
    }
    
    [Fact]
    public async Task Add_pet_to_volunteer2()
    {
        //Factory.SetupSuccessFileServiceMock();
        
        // arrange
        var volunteerId = await SeedVolunteer();
        var speciesId = await SeedSpecies();
        var breedId = await SeedBreed(speciesId);
        var command = Fixture.AddPetCommand(volunteerId);
        command.PetClassificationDto.SpeciesId = speciesId;
        command.PetClassificationDto.BreedId = breedId;
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        var voluntees = await WriteDbContext.Volunteers.ToListAsync();

        volunteer!.AllOwnedPets.Should().NotBeEmpty();
        volunteer!.AllOwnedPets.First().Id.Should().Be(result.Value);
        voluntees.Count().Should().Be(1);
    }
}