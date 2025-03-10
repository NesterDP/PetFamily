using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Files;
using PetFamily.Application.Files.FilesData;
using PetFamily.Application.Volunteers.Commands.AddPet;
using PetFamily.Domain.SpeciesContext.Entities;
using PetFamily.Domain.SpeciesContext.ValueObjects.BreedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class AddPetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, AddPetCommand> _sut;
    public AddPetHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, AddPetCommand>>();
    }

    [Fact]
    public async Task AddPet_success_should_add_pet_to_petless_volunteer()
    {
        // arrange
        var volunteer = await DataGenerator.SeedVolunteer(WriteDbContext);
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed = await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var petClassificationDto = new PetClassificationDto(species.Id, breed.Id);
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        
        // pet was added
        volunteer!.AllOwnedPets.Count.Should().Be(1);
        volunteer!.AllOwnedPets[0].Id.Should().Be(result.Value);
        
        // single pet should have first position
        volunteer!.AllOwnedPets[0].Position.Value.Should().Be(1);
    }
    
    [Fact]
    public async Task AddPet_success_should_add_pet_to_non_petless_volunteer()
    {
        // arrange
        var PET_COUNT = 5;
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");
        
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed= await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, species.Id, breed.Id);
        var petClassificationDto = new PetClassificationDto(species.Id, breed.Id);
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        
        // pet was added
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT + 1);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value).Should().NotBeNull();
        
        // added pet should have the last position
        volunteer!.AllOwnedPets
            .FirstOrDefault(p => p.Address.House == addressDto.House)!
            .Position.Value.Should().Be(PET_COUNT + 1);
    }
    
    [Fact]
    public async Task AddPet_failure_should_get_error_while_trying_to_add_pet_with_unknown_speciesId()
    {
        // arrange
        var PET_COUNT = 5;
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");
        
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed= await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, species.Id, breed.Id);
        var petClassificationDto = new PetClassificationDto(SpeciesId.NewSpeciesId(), breed.Id);
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        
        // pet wasn't added
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Address.House == addressDto.House).Should().BeNull();
    }
    
    [Fact]
    public async Task AddPet_failure_should_get_error_while_trying_to_add_pet_with_unknown_breedId()
    {
        // arrange
        var PET_COUNT = 5;
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");
        
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed= await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, species.Id, breed.Id);
        var petClassificationDto = new PetClassificationDto(species.Id, BreedId.NewBreedId());
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        
        // pet wasn't added
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Address.House == addressDto.House).Should().BeNull();
    }
    
    [Fact]
    public async Task AddPet_failure_should_get_error_while_trying_to_add_pet_with_known_but_unmatching_petClassification()
    {
        // arrange
        var PET_COUNT = 5;
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");
        
        var species1 = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed1 = await DataGenerator.SeedBreed(WriteDbContext, species1.Id);
        var species2 = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed2 = await DataGenerator.SeedBreed(WriteDbContext, species2.Id);
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, species1.Id, breed1.Id);
        var petClassificationDto = new PetClassificationDto(species1.Id, breed2.Id);
        var command = Fixture.AddPetCommand(volunteer.Id, petClassificationDto, addressDto);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();

        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        
        // pet wasn't added
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Address.House == addressDto.House).Should().BeNull();
    }
}