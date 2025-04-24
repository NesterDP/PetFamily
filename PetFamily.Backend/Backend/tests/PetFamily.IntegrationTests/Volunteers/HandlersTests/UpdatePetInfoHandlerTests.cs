using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;
using PetFamily.Volunteers.Application.Commands.UpdatePetInfo;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class UpdatePetInfoHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, UpdatePetInfoCommand> _sut;

    public UpdatePetInfoHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdatePetInfoCommand>>();
    }


    [Fact]
    public async Task UpdatePetInfo_success_should_update_all_matching_fields_from_updatePetInfo()
    {
        // arrange
        int PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, PET_COUNT, species.Id, breed.Id);
        var pet = volunteer.AllOwnedPets[0];

        const string name = "Test Pet";
        const string description = "Test description";
        var petClassificationDto = new PetClassificationDto(species.Id, breed.Id);
        const string color = "red";
        const string healthInfo = "Test Health Info";
        var addressDto = new AddressDto("Moscow", "Lenina 12", "123");
        const float weight = (float)5.12;
        const float height = (float)23.43;
        const string ownerPhoneNumber = "1-2-333-44-55-66";
        const bool isCastrated = false;
        var dateOfBirth = DateTime.UtcNow.AddYears(-5);
        const bool isVaccinated = true;
        List<TransferDetailDto> transferDetails =
        [
            new TransferDetailDto("mir", "for transfers within country"),
            new TransferDetailDto("visa", "for transfers outside of country")
        ];
        var command = new UpdatePetInfoCommand(
            volunteer.Id,
            pet.Id,
            name,
            description,
            petClassificationDto,
            color,
            healthInfo,
            addressDto,
            weight,
            height,
            ownerPhoneNumber,
            isCastrated,
            dateOfBirth,
            isVaccinated,
            transferDetails);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        var updatedPet = updatedVolunteer.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);

        // all data is updated correctly
        updatedPet.Name.Value.Should().Be(name);
        updatedPet.Description.Value.Should().Be(description);
        updatedPet.PetClassification.SpeciesId.Should().Be(species.Id);
        updatedPet.PetClassification.BreedId.Should().Be(breed.Id);
        updatedPet.Color.Value.Should().Be(color);
        updatedPet.HealthInfo.Value.Should().Be(healthInfo);
        updatedPet.Address.City.Should().Be(addressDto.City);
        updatedPet.Address.House.Should().Be(addressDto.House);
        updatedPet.Address.Apartment.Should().Be(addressDto.Apartment);
        updatedPet.Weight.Value.Should().Be(weight);
        updatedPet.Height.Value.Should().Be(height);
        updatedPet.OwnerPhoneNumber.Value.Should().Be(ownerPhoneNumber);
        updatedPet.IsCastrated.Value.Should().Be(isCastrated);
        updatedPet.DateOfBirth.Value.Should().Be(dateOfBirth);
        updatedPet.IsVaccinated.Value.Should().Be(isVaccinated);

        updatedPet!.TransferDetailsList[0].Name.Should().Be(transferDetails[0].Name);
        updatedPet!.TransferDetailsList[0].Description.Should().Be(transferDetails[0].Description);
        updatedPet!.TransferDetailsList[1].Name.Should().Be(transferDetails[1].Name);
        updatedPet!.TransferDetailsList[1].Description.Should().Be(transferDetails[1].Description);
    }

    [Fact]
    public async Task UpdatePetInfo_failure_should_return_error_because_of_breedId_validation()
    {
        // arrange
        int PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, PET_COUNT, species.Id, breed.Id);
        var pet = volunteer.AllOwnedPets[0];


        var petClassificationDto = new PetClassificationDto(species.Id, Guid.NewGuid());
        var command = CreateUpdatePetCommand(volunteer.Id, pet.Id, petClassificationDto);
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePetInfo_failure_should_return_error_because_of_speciesId_validation()
    {
        // arrange
        int PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, PET_COUNT, species.Id, breed.Id);
        var pet = volunteer.AllOwnedPets[0];

        var petClassificationDto = new PetClassificationDto(Guid.NewGuid(), breed.Id);
        var command = CreateUpdatePetCommand(volunteer.Id, pet.Id, petClassificationDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
        ;
    }

    [Fact]
    public async Task UpdatePetInfo_failure_should_return_error_because_of_unmatching_breedId_and_species_id()
    {
        // arrange
        int PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var species2 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed2 = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species2.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, PET_COUNT, species.Id, breed.Id);
        var pet = volunteer.AllOwnedPets[0];

        var petClassificationDto = new PetClassificationDto(species.Id, breed2.Id);
        var command = CreateUpdatePetCommand(volunteer.Id, pet.Id, petClassificationDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePetInfo_failure_should_return_error_because_of_unmatching_breedId_and_species_id_reverse()
    {
        // arrange
        int PET_COUNT = 5;
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var species2 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed2 = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species2.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, PET_COUNT, species.Id, breed.Id);
        var pet = volunteer.AllOwnedPets[0];

        var petClassificationDto = new PetClassificationDto(species2.Id, breed.Id);
        var command = CreateUpdatePetCommand(volunteer.Id, pet.Id, petClassificationDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }

    private UpdatePetInfoCommand CreateUpdatePetCommand(
        Guid volunteerId,
        Guid speciesId,
        PetClassificationDto petClassification)
    {
        const string name = "Test Pet";
        const string description = "Test description";
        var petClassificationDto = petClassification;
        const string color = "red";
        const string healthInfo = "Test Health Info";
        var addressDto = new AddressDto("Moscow", "Lenina 12", "123");
        const float weight = (float)5.12;
        const float height = (float)23.43;
        const string ownerPhoneNumber = "1-2-333-44-55-66";
        const bool isCastrated = false;
        var dateOfBirth = DateTime.UtcNow.AddYears(-5);
        const bool isVaccinated = true;
        List<TransferDetailDto> transferDetails =
        [
            new TransferDetailDto("mir", "for transfers within country"),
            new TransferDetailDto("visa", "for transfers outside of country")
        ];
        var command = new UpdatePetInfoCommand(
            volunteerId,
            speciesId,
            name,
            description,
            petClassificationDto,
            color,
            healthInfo,
            addressDto,
            weight,
            height,
            ownerPhoneNumber,
            isCastrated,
            dateOfBirth,
            isVaccinated,
            transferDetails);

        return command;
    }
}