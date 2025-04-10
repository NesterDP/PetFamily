using AutoFixture;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;
using PetFamily.Volunteers.Application.Commands.AddPet;
using PetFamily.Volunteers.Application.Commands.DeletePetPhotos;
using PetFamily.Volunteers.Application.Commands.UploadPhotosToPet;

namespace PetFamily.IntegrationTests.General;

public static class FixtureExtensions
{
    public static AddPetCommand AddPetCommand(
        this Fixture fixture,
        Guid volunteerId,
        PetClassificationDto petClassificationDto,
        AddressDto addressDto)
    {
        return fixture.Build<AddPetCommand>()
            .With(c => c.VolunteerId, volunteerId)
            .With(c => c.Name, "Test Name")
            .With(c => c.Description, "Test Description")
            .With(c => c.PetClassificationDto, petClassificationDto)
            .With(c => c.Color, "test color")
            .With(c => c.HealthInfo, "test health info")
            .With(c => c.AddressDto, addressDto)
            .With(c => c.Weight, 12.4)
            .With(c => c.Height, 10.5)
            .With(c => c.OwnerPhoneNumber, "8-9-222-32-12-32")
            .With(c => c.DateOfBirth, DateTime.UtcNow.AddYears(-5))
            .With(c => c.HelpStatus, "InSearchOfHome")
            .Create();
    }
    public static UploadPhotosToPetCommand UploadPhotosToPetCommand(
        this Fixture fixture,
        Guid volunteerId,
        Guid petId)
    {
        return fixture.Build<UploadPhotosToPetCommand>()
            .With(c => c.VolunteerId, volunteerId)
            .With(c => c.PetId, petId)
            .Create();
    }
}