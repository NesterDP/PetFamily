using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Volunteers.Commands.UpdatePetMainPhoto;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class UpdatePetMainPhotoHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid,  UpdatePetMainPhotoCommand> _sut;

    public UpdatePetMainPhotoHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdatePetMainPhotoCommand>>();
        Fixture.Inject<Stream>(Stream.Null);
    }

    [Fact]
    public async Task UploadPetMainPhoto_success_should_make_selected_photo_main_if_there_were_no_main_photo()
    {
        // arrange
        List<Photo> photos =
        [
            new Photo(FilePath.Create("new_photo_1.jpg").Value),
            new Photo(FilePath.Create("new_photo_2.jpg").Value),
            new Photo(FilePath.Create("new_photo_3.jpg").Value)
        ];
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(photos);
        await WriteDbContext.SaveChangesAsync();
        var NEW_MAIN_PHOTO = "new_photo_2.jpg";
        
        var command = new UpdatePetMainPhotoCommand(volunteer.Id, pet.Id, NEW_MAIN_PHOTO);
        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);
        
        // photo actually is main while the rest are not
        var main_photos = pet.PhotosList.Where(photo => photo.Main == true).ToList();
        main_photos.Count.Should().Be(1);
        main_photos[0].PathToStorage.Path.Should().Be(NEW_MAIN_PHOTO);
    }
    
    [Fact]
    public async Task UploadPetMainPhoto_success_should_make_selected_photo_main_if_there_were_main_photo()
    {
        // arrange
        List<Photo> photos =
        [
            new Photo(FilePath.Create("new_photo_1.jpg").Value),
            new Photo(FilePath.Create("new_photo_2.jpg").Value),
            new Photo(FilePath.Create("new_photo_3.jpg").Value)
        ];
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(photos);
        pet.UpdateMainPhoto(photos[0]);
        await WriteDbContext.SaveChangesAsync();
        var NEW_MAIN_PHOTO = "new_photo_3.jpg";
        
        var command = new UpdatePetMainPhotoCommand(volunteer.Id, pet.Id, NEW_MAIN_PHOTO);
        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);
        
        // photo actually is main while the rest are not
        var main_photos = pet.PhotosList.Where(photo => photo.Main == true).ToList();
        main_photos.Count.Should().Be(1);
        main_photos[0].PathToStorage.Path.Should().Be(NEW_MAIN_PHOTO);
    }
    
    [Fact]
    public async Task UploadPetMainPhoto_failure_requested_main_photo_doesnt_exist_in_database()
    {
        // arrange
        List<Photo> photos =
        [
            new Photo(FilePath.Create("new_photo_1.jpg").Value),
            new Photo(FilePath.Create("new_photo_2.jpg").Value)
        ];
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(photos);
        pet.UpdateMainPhoto(photos[0]);
        await WriteDbContext.SaveChangesAsync();
        var NEW_MAIN_PHOTO = "new_photo_3.jpg";
        var OLD_MAIN_PHOTO = "new_photo_1.jpg";
        
        var command = new UpdatePetMainPhotoCommand(volunteer.Id, pet.Id, NEW_MAIN_PHOTO);
        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsFailure.Should().BeTrue();
        
        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == pet.Id);
        
        // first photo is still the main one
        var main_photos = pet.PhotosList.Where(photo => photo.Main == true).ToList();
        main_photos.Count.Should().Be(1);
        main_photos[0].PathToStorage.Path.Should().Be(OLD_MAIN_PHOTO);
    }
}