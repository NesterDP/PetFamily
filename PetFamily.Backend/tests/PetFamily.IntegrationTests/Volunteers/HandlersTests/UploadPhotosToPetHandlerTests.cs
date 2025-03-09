using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Volunteers.Commands.UploadPhotosToPet;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class UploadPhotosToPetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid,  UploadPhotosToPetCommand> _sut;

    public UploadPhotosToPetHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UploadPhotosToPetCommand>>();
        Fixture.Inject<Stream>(Stream.Null);
    }

    [Fact]
    public async Task UploadPhotos_success_should_add_photos_to_photoless_pet_in_database()
    {
        // arrange
        Factory.SetupSuccessFileServiceMock();
        var PET_COUNT = 5;
        
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        var command = Fixture.UploadPhotosToPetCommand(volunteer.Id, pet.Id);
        
        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);
        pet.PhotosList.Count.Should().BeGreaterThan(0); // photos are actually added
    }
    
    [Fact]
    public async Task UploadPhotos_success_should_add_photos_to_non_photoless_pet_in_database()
    {
        // arrange
        Factory.SetupSuccessFileServiceMock();
        List<string> petPhotos = ["photo1", "photo2", "photo3"];
        var PET_COUNT = 5;
        
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotos.Select(p => new Photo(FilePath.Create(p).Value)));
        pet.UpdateMainPhoto(pet.PhotosList[0]); // first photo is main one
        await WriteDbContext.SaveChangesAsync();
        var command = Fixture.UploadPhotosToPetCommand(volunteer.Id, pet.Id);
        
        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);
        pet.PhotosList.Count.Should().BeGreaterThan(petPhotos.Count); // photos are actually added
        
        // existed photos shouldn't be affected
        petPhotos.All(photoName => pet.PhotosList.Any(photo => photo.PathToStorage.Path == photoName)).Should().BeTrue();
        
        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.PathToStorage.Path == petPhotos[0] && photo.Main == true).Should().BeTrue();
    }
    
    [Fact]
    public async Task UploadPhotos_failure_should_return_error_while_not_affecting_database()
    {
        // arrange
        Factory.SetupFailureFileServiceMock();
        List<string> petPhotos = ["photo1", "photo2", "photo3"];
        var PET_COUNT = 5;
        
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotos.Select(p => new Photo(FilePath.Create(p).Value)));
        pet.UpdateMainPhoto(pet.PhotosList[0]); // first photo is main one
        await WriteDbContext.SaveChangesAsync();
        var command = Fixture.UploadPhotosToPetCommand(volunteer.Id, pet.Id);
        
        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsFailure.Should().BeTrue();
        
        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == pet.Id);
        pet.PhotosList.Count.Should().Be(petPhotos.Count);
        
        // existed photos shouldn't be affected
        petPhotos.All(photoName => pet.PhotosList.Any(photo => photo.PathToStorage.Path == photoName)).Should().BeTrue();
        
        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.PathToStorage.Path == petPhotos[0] && photo.Main == true).Should().BeTrue();
    }
}