using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.ValueObjects;
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
        List<FilePath> photosForDeletion =
        [
            FilePath.Create("photo1").Value,
            FilePath.Create("photo3").Value
        ];
        Factory.SetupSuccessFileServiceMock(photosForDeletion);
        List<string> petPhotos = ["photo1", "photo2", "photo3"];
        var PET_COUNT = 5;
        
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotos.Select(p => new Photo(FilePath.Create(p).Value)));
        pet.UpdateMainPhoto(pet.PhotosList[1]); // "photo2" is main one
        await VolunteersWriteDbContext.SaveChangesAsync();
        
        // expected result
        var resultList = petPhotos.Except(photosForDeletion.Select(p => p.Path)).ToList();
        var command = Fixture.DeletePetPhotosCommand(
            volunteer.Id,
            pet.Id,
            photosForDeletion.Select(d => d.Path).ToList());
        
        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value);
        
        // size equality
        pet.PhotosList.Count.Should().Be(resultList.Count);
        
        // position independent content equality
        resultList.All(photoName => pet.PhotosList.Any(photo => photo.PathToStorage.Path == photoName)).Should().BeTrue();
        
        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.PathToStorage.Path == petPhotos[1] && photo.Main == true).Should().BeTrue();
    }
    
    [Fact]
    public async Task DeletePhotos_failure_should_remove_selected_photos_from_database_while_returning_error()
    {
        // arrange
        Factory.SetupFailureFileServiceMock();
        var PET_COUNT = 5;
        List<string> petPhotos = ["photo1", "photo2", "photo3"];
        List<string> photosForDeletion = ["photo1", "photo3"];
        
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        pet.UpdatePhotos(petPhotos.Select(p => new Photo(FilePath.Create(p).Value)));
        pet.UpdateMainPhoto(pet.PhotosList[1]); // "photo2" is main one
        await VolunteersWriteDbContext.SaveChangesAsync();
        
        // expected result
        var resultList = petPhotos.Except(photosForDeletion).ToList();
        var command = Fixture.DeletePetPhotosCommand(volunteer.Id, pet.Id, photosForDeletion);
        
        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsFailure.Should().BeTrue();
        
        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        pet = volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == pet.Id);
        
        // size equality
        pet.PhotosList.Count.Should().Be(resultList.Count);
        
        // position independent equality
        resultList.All(photoName => pet.PhotosList.Any(photo => photo.PathToStorage.Path == photoName)).Should().BeTrue();
        
        // main photo shouldn't be affected
        pet.PhotosList.Any(photo => photo.PathToStorage.Path == petPhotos[1] && photo.Main == true).Should().BeTrue();
    }
}