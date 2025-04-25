using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Application.Commands.AddPet;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class AddPetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, AddPetCommand> _sut;

    public AddPetHandlerTests(VolunteerTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, AddPetCommand>>();
    }

    [Fact]
    public async Task AddPet_success_should_add_pet_to_petless_volunteer()
    {
        // arrange
        var volunteer = await DataGenerator.SeedVolunteer(VolunteersWriteDbContext);
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var petClassificationDto = new PetClassificationDto(species.Id, breed.Id);
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();

        // pet was added
        volunteer!.AllOwnedPets.Count.Should().Be(1);
        volunteer.AllOwnedPets[0].Id.Should().Be(result.Value);

        // single pet should have first position
        volunteer.AllOwnedPets[0].Position.Value.Should().Be(1);
    }

    [Fact]
    public async Task AddPet_success_should_add_pet_to_non_petless_volunteer()
    {
        // arrange
        const int PET_COUNT = 5;
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");

        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(
            VolunteersWriteDbContext, PET_COUNT, species.Id, breed.Id);
        var petClassificationDto = new PetClassificationDto(species.Id, breed.Id);
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);

        // pet was added
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT + 1);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value).Should().NotBeNull();

        // added pet should have the last position
        volunteer.AllOwnedPets
            .FirstOrDefault(p => p.Address.House == addressDto.House)!
            .Position.Value.Should().Be(PET_COUNT + 1);
    }

    [Fact]
    public async Task AddPet_failure_should_get_error_while_trying_to_add_pet_with_unknown_speciesId()
    {
        // arrange
        const int PET_COUNT = 5;
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");

        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(
            VolunteersWriteDbContext, PET_COUNT, species.Id, breed.Id);
        var petClassificationDto = new PetClassificationDto(SpeciesId.NewSpeciesId(), breed.Id);
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);

        // pet wasn't added
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Address.House == addressDto.House).Should().BeNull();
    }

    [Fact]
    public async Task AddPet_failure_should_get_error_while_trying_to_add_pet_with_unknown_breedId()
    {
        // arrange
        const int PET_COUNT = 5;
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");

        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(
            VolunteersWriteDbContext, PET_COUNT, species.Id, breed.Id);
        var petClassificationDto = new PetClassificationDto(species.Id, BreedId.NewBreedId());
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);

        // pet wasn't added
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Address.House == addressDto.House).Should().BeNull();
    }

    [Fact]
    public async Task
        AddPet_failure_should_get_error_while_trying_to_add_pet_with_known_but_unmatching_petClassification()
    {
        // arrange
        const int PET_COUNT = 5;
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");

        var species1 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed1 = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species1.Id);
        var species2 = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed2 = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species2.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(
            VolunteersWriteDbContext, PET_COUNT, species1.Id, breed1.Id);
        var petClassificationDto = new PetClassificationDto(species1.Id, breed2.Id);
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);

        // pet wasn't added
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT);
        volunteer.AllOwnedPets.FirstOrDefault(p => p.Address.House == addressDto.House).Should().BeNull();
    }
}