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
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class AddPetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, AddPetCommand> _sut;
    private readonly IFilesProvider _filesProvider;

    public AddPetHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, AddPetCommand>>();
        _filesProvider = Scope.ServiceProvider.GetRequiredService<IFilesProvider>();
    }

    [Fact]
    public async Task Add_pet_to_volunteer_that_doesnt_have_pets()
    {
        /*Factory.SetupFailureFileServiceMock();
        var filesResult = await _filesProvider.UploadFiles(new List<FileData>());*/
        /*filesResult.IsSuccess.Should().BeFalse();*/

        // arrange
        var volunteerId = await DataGenerator.SeedVolunteer(WriteDbContext);
        var speciesId = await DataGenerator.SeedSpecies(WriteDbContext);
        var breedId = await DataGenerator.SeedBreed(WriteDbContext, speciesId);
        var petClassificationDto = new PetClassificationDto(speciesId, breedId);
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");
        var command = Fixture.AddPetCommand(volunteerId, petClassificationDto, addressDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        volunteer!.AllOwnedPets.Should().NotBeEmpty();
        volunteer!.AllOwnedPets[0].Id.Should().Be(result.Value);
    }
    
    [Fact]
    public async Task Add_pet_to_volunteer_that_have_pets()
    {
        var PET_COUNT = 5;
        // arrange
        var speciesId = await DataGenerator.SeedSpecies(WriteDbContext);
        var breedId = await DataGenerator.SeedBreed(WriteDbContext, speciesId);
        var volunteerId = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT, speciesId, breedId);
        var petClassificationDto = new PetClassificationDto(speciesId, breedId);
        var addressDto = new AddressDto("Moscow", "Lenina 14", "123");
        var command = Fixture.AddPetCommand(volunteerId, petClassificationDto, addressDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteerId);
        volunteer!.AllOwnedPets.Count.Should().Be(PET_COUNT + 1);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value).Should().NotBeNull();
    }
}