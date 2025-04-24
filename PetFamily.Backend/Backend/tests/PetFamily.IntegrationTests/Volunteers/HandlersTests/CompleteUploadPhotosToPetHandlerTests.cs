using AutoFixture;
using FileService.Contracts.Responses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Application.Commands.CompleteUploadPhotosToPet;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class CompleteUploadPhotosToPetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<CompleteMultipartUploadResponse, CompleteUploadPhotosToPetCommand> _sut;

    public CompleteUploadPhotosToPetHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<ICommandHandler<CompleteMultipartUploadResponse, CompleteUploadPhotosToPetCommand>>();
    }

    [Fact]
    public async Task CompleteUploadPhotos_success_should_add_photos_to_photoless_pet_in_database()
    {
        // arrange
        int PET_COUNT = 5;
        int UPLOADED_PHOTOS_COUNT = 4;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        
        var command = new CompleteUploadPhotosToPetCommand(
            volunteer.Id,
            pet.Id,
            Fixture.CreateMany<CompleteUploadFileDto>(UPLOADED_PHOTOS_COUNT).ToList());
        
        Factory.SetupSuccessCompleteMultipartMock(Fixture.CreateMany<Guid>(UPLOADED_PHOTOS_COUNT).ToList());

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MultipartCompleteInfos.Count.Should().Be(UPLOADED_PHOTOS_COUNT);

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == pet.Id);

        // photos are actually added
        pet.PhotosList.Count.Should().Be(UPLOADED_PHOTOS_COUNT);
        result.Value.MultipartCompleteInfos.All(p => 
            pet.PhotosList.Any(photo => photo.Id == p.FileId)).Should().BeTrue();
    }

    [Fact]
    public async Task CompleteUploadPhotos_success_should_add_photos_to_non_photoless_pet_in_database()
    {
        // arrange
        List<Guid> petPhotos = [Guid.NewGuid(), Guid.NewGuid()];
        int PET_COUNT = 5;
        int UPLOADED_PHOTOS_COUNT = 4;

        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotos.Select(p => Photo.Create(p, Photo.AllowedTypes.First()).Value));
        pet.UpdateMainPhoto(pet.PhotosList[0]); // "[0]" is main one
        await VolunteersWriteDbContext.SaveChangesAsync();
        
        var command = new CompleteUploadPhotosToPetCommand(
            volunteer.Id,
            pet.Id,
            Fixture.CreateMany<CompleteUploadFileDto>(UPLOADED_PHOTOS_COUNT).ToList());
        
        Factory.SetupSuccessCompleteMultipartMock(Fixture.CreateMany<Guid>(UPLOADED_PHOTOS_COUNT).ToList());

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MultipartCompleteInfos.Count.Should().Be(UPLOADED_PHOTOS_COUNT);

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == pet.Id);

        // photos are actually added
        pet!.PhotosList.Count.Should().Be(petPhotos.Count + result.Value.MultipartCompleteInfos.Count);
        result.Value.MultipartCompleteInfos
            .All(p => pet.PhotosList.Any(photo => photo.Id == p.FileId))
            .Should().BeTrue();

        // existed photos shouldn't be affected
        petPhotos.All(photoId => pet.PhotosList.Any(photo => photo.Id == photoId)).Should().BeTrue();

        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.Id == petPhotos[0] && photo.Main == true).Should().BeTrue();
    }

    [Fact]
    public async Task CompleteUploadPhotos_failure_should_return_error_while_not_affecting_database()
    {
        // arrange
        List<Guid> petPhotos = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        int PET_COUNT = 5;
        int UPLOADED_PHOTOS_COUNT = 4;

        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotos.Select(p => Photo.Create(p, Photo.AllowedTypes.First()).Value));
        pet.UpdateMainPhoto(pet.PhotosList[0]); // "[0]" is main one
        await VolunteersWriteDbContext.SaveChangesAsync();
        
        var command = new CompleteUploadPhotosToPetCommand(
            volunteer.Id,
            pet.Id,
            Fixture.CreateMany<CompleteUploadFileDto>(UPLOADED_PHOTOS_COUNT).ToList());
        
        Factory.SetupFailureCompleteMultipartMock();

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