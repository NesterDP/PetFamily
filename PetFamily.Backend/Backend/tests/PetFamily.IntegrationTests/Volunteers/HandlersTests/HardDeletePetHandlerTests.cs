using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Volunteers.Application.Commands.DeletePet;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class HardDeletePetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, DeletePetCommand> _sut;
    public HardDeletePetHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<HardDeletePetHandler>();
    }
    
    [Fact]
    public async Task HardDelete_success_should_hard_delete_pet_who_has_first_position()
    {
        // arrange
        var PET_COUNT = 5;
        var POSITION_OF_DELETED = 1;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == POSITION_OF_DELETED);
        var secondPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 2);
        var thirdPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 3);
        var fourthPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 4);
        var fifthPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 5);
        var command = new DeletePetCommand(volunteer.Id, pet.Id);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value)!.Should().BeNull();

        // position of each pet are adjusted
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == secondPet.Id).Position.Value.Should().Be(1);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == thirdPet.Id).Position.Value.Should().Be(2);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == fourthPet.Id).Position.Value.Should().Be(3);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == fifthPet.Id).Position.Value.Should().Be(4);
    }
    
    [Fact]
    public async Task HardDelete_success_should_hard_delete_pet_who_has_last_position()
    {
        // arrange
        var PET_COUNT = 5;
        var POSITION_OF_DELETED = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == POSITION_OF_DELETED);
        var firstPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 1);
        var secondPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 2);
        var thirdPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 3);
        var fourthPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 4);
        var command = new DeletePetCommand(volunteer.Id, pet.Id);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value)!.Should().BeNull();

        // position of each pet are adjusted
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == firstPet.Id).Position.Value.Should().Be(1);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == secondPet.Id).Position.Value.Should().Be(2);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == thirdPet.Id).Position.Value.Should().Be(3);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == fourthPet.Id).Position.Value.Should().Be(4);
    }
    
    [Fact]
    public async Task HardDelete_success_should_hard_delete_pet_who_is_not_in_border_position()
    {
        // arrange
        var PET_COUNT = 5;
        var POSITION_OF_DELETED = 3;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == POSITION_OF_DELETED);
        var firstPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 1);
        var secondPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 2);
        var fourthPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 4);
        var fifthPet = volunteer.AllOwnedPets.FirstOrDefault(p => p.Position.Value == 5);
        var command = new DeletePetCommand(volunteer.Id, pet.Id);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value)!.Should().BeNull();

        // position of each pet are adjusted
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == firstPet.Id).Position.Value.Should().Be(1);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == secondPet.Id).Position.Value.Should().Be(2);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == fourthPet.Id).Position.Value.Should().Be(3);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == fifthPet.Id).Position.Value.Should().Be(4);
    }
}