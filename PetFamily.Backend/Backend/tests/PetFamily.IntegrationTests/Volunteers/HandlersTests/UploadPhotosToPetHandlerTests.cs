using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Application.Commands.UploadPhotosToPet;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class UploadPhotosToPetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, UploadPhotosToPetCommand> _sut;

    public UploadPhotosToPetHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UploadPhotosToPetCommand>>();
        Fixture.Inject<Stream>(Stream.Null);
    }

    [Fact]
    public async Task UploadPhotos_success_should_add_photos_to_photoless_pet_in_database()
    {
        // arrange
        
        // TODO: здесь должен будет быть обновленный Mock для взаимодействия с FileService
        // Factory.SetupSuccessFileServiceMock(photosForUpload);
        
        List<Guid> photosForUpload = [Guid.NewGuid(), Guid.NewGuid()];
        var PET_COUNT = 5;

        var volunteer =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        var command = Fixture.UploadPhotosToPetCommand(volunteer.Id, pet.Id);

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);

        // photos are actually added
        pet.PhotosList.Count.Should().Be(photosForUpload.Count);
        photosForUpload.All(p => pet.PhotosList.Any(photo => photo.Id == p)).Should().BeTrue();
    }

    [Fact]
    public async Task UploadPhotos_success_should_add_photos_to_non_photoless_pet_in_database()
    {
        // arrange
        
        // TODO: здесь должен будет быть обновленный Mock для взаимодействия с FileService
        // Factory.SetupSuccessFileServiceMock(photosForUpload);
        
        List<Guid> photosForUpload = [Guid.NewGuid(), Guid.NewGuid()];

        List<Guid> petPhotos = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var PET_COUNT = 5;

        var volunteer =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotos.Select(p => Photo.Create(p, Photo.AllowedTypes.First()).Value));
        pet.UpdateMainPhoto(pet.PhotosList[0]); // "[0]" is main one
        await VolunteersWriteDbContext.SaveChangesAsync();
        var command = Fixture.UploadPhotosToPetCommand(volunteer.Id, pet.Id);

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);

        // photos are actually added
        pet!.PhotosList.Count.Should().Be(petPhotos.Count + photosForUpload.Count);
        photosForUpload.All(photoId => pet.PhotosList.Any(photo => photo.Id == photoId)).Should().BeTrue();

        // existed photos shouldn't be affected
        petPhotos.All(photoId => pet.PhotosList.Any(photo => photo.Id == photoId)).Should().BeTrue();

        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.Id == petPhotos[0] && photo.Main == true).Should().BeTrue();
    }

    [Fact]
    public async Task UploadPhotos_failure_should_return_error_while_not_affecting_database()
    {
        // arrange
        
        // TODO: здесь должен будет быть обновленный Mock для взаимодействия с FileService
        // Factory.SetupFailureFileServiceMock();
        
        List<Guid> petPhotos = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        var PET_COUNT = 5;

        var volunteer =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotos.Select(p => Photo.Create(p, Photo.AllowedTypes.First()).Value));
        pet.UpdateMainPhoto(pet.PhotosList[0]); // "photo1" is main one
        await VolunteersWriteDbContext.SaveChangesAsync();
        var command = Fixture.UploadPhotosToPetCommand(volunteer.Id, pet.Id);

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsFailure.Should().BeTrue();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == pet.Id);

        // photos count is unchanged
        pet.PhotosList.Count.Should().Be(petPhotos.Count);

        // existed photos shouldn't be affected
        petPhotos.All(photoId => pet.PhotosList.Any(photo => photo.Id == photoId)).Should().BeTrue();

        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.Id == petPhotos[0] && photo.Main == true).Should().BeTrue();
    }
}