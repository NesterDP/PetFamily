using AutoFixture;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Volunteers.Commands.AddPet;
using PetFamily.Application.Volunteers.Commands.Create;

namespace PetFamily.IntegrationTests.General;

public static class FixtureExtensions
{
    public static CreateVolunteerCommand CreateVolunteerCommand(
        this Fixture fixture)
    {
        return fixture.Build<CreateVolunteerCommand>()
            .With(c => c.VolunteerCommandDto.PhoneNumber, "8-9-222-32-12-32")
            .With(c => c.VolunteerCommandDto.Email, "test@test.com")
            .Create();
    }
    
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
            .With( c => c.PetClassificationDto, petClassificationDto)
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
}