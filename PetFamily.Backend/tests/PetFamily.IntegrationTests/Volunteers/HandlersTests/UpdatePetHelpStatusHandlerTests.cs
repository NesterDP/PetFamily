using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Volunteers.Commands.UpdatePetInfo;
using PetFamily.Application.Volunteers.Commands.UpdatePetStatus;
using PetFamily.Domain.VolunteerManagment.ValueObjects.PetVO;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;


public class UpdatePetHelpStatusHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, UpdatePetHelpStatusCommand> _sut;

    public UpdatePetHelpStatusHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdatePetHelpStatusCommand>>();
    }
    
    [Fact]
    public async Task UpdatePetHelpStatus_success_should_change_help_status_of_pet_to_UnderMedicalTreatment()
    {
        // arrange
        var PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed = await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, species.Id, breed.Id);
        var pet = volunteer.AllOwnedPets[0];
        const string NEW_HELP_STATUS = "UnderMedicalTreatment";

        var command = new UpdatePetHelpStatusCommand(volunteer.Id, pet.Id, NEW_HELP_STATUS);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        var updatedPet = updatedVolunteer.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);

        // all data is updated correctly
        updatedPet.HelpStatus.Value.Should().Be(PetStatus.UnderMedicalTreatment);
    }
    
    [Fact]
    public async Task UpdatePetHelpStatus_success_should_change_help_status_of_pet_to_InSearchOfHome()
    {
        // arrange
        var PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed = await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, species.Id, breed.Id);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdateHelpStatus(HelpStatus.Create("UnderMedicalTreatment").Value);
        await WriteDbContext.SaveChangesAsync();
        const string NEW_HELP_STATUS = "InSearchOfHome";

        var command = new UpdatePetHelpStatusCommand(volunteer.Id, pet.Id, NEW_HELP_STATUS);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        var updatedPet = updatedVolunteer.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);

        // all data is updated correctly
        updatedPet.HelpStatus.Value.Should().Be(PetStatus.InSearchOfHome);
    }
    
    [Fact]
    public async Task UpdatePetHelpStatus_failure_should_return_error_because_of_not_passing_validation()
    {
        // arrange
        var PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed = await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, species.Id, breed.Id);
        var pet = volunteer.AllOwnedPets[0];
        const string NEW_HELP_STATUS = "incorrect status";

        var command = new UpdatePetHelpStatusCommand(volunteer.Id, pet.Id, NEW_HELP_STATUS);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        var updatedPet = updatedVolunteer.AllOwnedPets.FirstOrDefault(p => p.Id == pet.Id);

        // should be unchanged
        updatedPet.HelpStatus.Value.Should().Be(PetStatus.InSearchOfHome);
    }
}