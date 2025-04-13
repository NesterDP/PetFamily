using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Application.Commands.DeletePetPhotos;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class DeletePetPhotosHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, DeletePetPhotosCommand> _sut;

    public DeletePetPhotosHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeletePetPhotosCommand>>();
    }

    [Fact]
    public async Task DeletePhotos_success_should_remove_selected_photos_from_database()
    {
        // arrange
        List<Guid> petPhotosIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        List<Guid> photosForDeletion = [petPhotosIds[0], petPhotosIds[2]];

        var PET_COUNT = 5;
        var volunteer =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotosIds.Select(p => Photo.Create(FileId.Create(p), Photo.AllowedTypes.First()).Value));
        pet.UpdateMainPhoto(pet.PhotosList[1]); // "[1]" is main one
        await VolunteersWriteDbContext.SaveChangesAsync();

        // expected results
        var expectedResultList = petPhotosIds.Except(photosForDeletion).ToList();
        var command = new DeletePetPhotosCommand(volunteer.Id, pet.Id, photosForDeletion);
        
        Factory.SetupSuccessDeleteFilesByIdsMock(photosForDeletion);

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);

        // size equality
        pet.PhotosList.Count.Should().Be(expectedResultList.Count);

        // position independent content equality
        expectedResultList.All(photoId => pet.PhotosList.Any(photo => photo.Id == photoId)).Should().BeTrue();

        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.Id == petPhotosIds[1] && photo.Main == true).Should().BeTrue();
    }

    [Fact]
    public async Task DeletePhotos_failure_should_not_remove_selected_photos_from_database_and_return_error()
    {
        // arrange
        var PET_COUNT = 5;
        List<Guid> petPhotosIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        List<Guid> photosForDeletion = [petPhotosIds[0], petPhotosIds[2]];

        var volunteer =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotosIds.Select(p => Photo.Create(FileId.Create(p), Photo.AllowedTypes.First()).Value));
        pet.UpdateMainPhoto(pet.PhotosList[1]); // "[1]" is main one
        await VolunteersWriteDbContext.SaveChangesAsync();

        // expected ResultListresult
        var expectedResultList = petPhotosIds.Except(photosForDeletion).ToList();
        var command = new DeletePetPhotosCommand(volunteer.Id, pet.Id, photosForDeletion);
        
        Factory.SetUpFailureDeleteFilesByIdsMock();

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsFailure.Should().BeTrue();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == pet.Id);

        // size equality
        pet.PhotosList.Count.Should().Be(expectedResultList.Count);

        // position independent equality
        expectedResultList.All(photoId => pet.PhotosList.Any(photo => photo.Id == photoId)).Should().BeTrue();

        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.Id == petPhotosIds[1] && photo.Main == true).Should().BeTrue();
    }
}