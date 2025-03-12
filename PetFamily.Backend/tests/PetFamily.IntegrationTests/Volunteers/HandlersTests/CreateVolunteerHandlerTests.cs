using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.Commands.Create;
using PetFamily.Domain.SpeciesContext.ValueObjects.BreedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class CreateVolunteerHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _sut;

    public CreateVolunteerHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
    }

    [Fact]
    public async Task CreateVolunteer_success_should_add_volunteer_to_database()
    {
        // arrange
        const string firstName = "Alexandr";
        const string lastName = "Volkov";
        const string surname = "Alexandrovich";
        const string email = "test@mail.com";
        const string phoneNumber = "1-2-333-44-55-66";
        const string description = "Test description";
        const int experience = 2;
        var fullName = new FullNameDto(firstName, lastName, surname);
        
        List<SocialNetworkDto> socialNetworks =
        [
            new SocialNetworkDto("vk", "vk.com"),
            new SocialNetworkDto("mail", "mail.com")
        ];
        
        List<TransferDetailDto> transferDetails =
        [
            new TransferDetailDto("mir", "for transfers within country"),
            new TransferDetailDto("visa", "for transfers outside of country")
        ];

        var volunteerCommandDto = new CreateVolunteerDto(
            fullName,
            email,
            phoneNumber,
            description,
            experience);
        
        var command = new CreateVolunteerCommand(volunteerCommandDto, socialNetworks, transferDetails);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value);

        // all data is bounded correctly
        volunteer.FullName.FirstName.Should().Be(firstName);
        volunteer.FullName.LastName.Should().Be(lastName);
        volunteer.FullName.Surname.Should().Be(surname);
        volunteer.Description.Value.Should().Be(description);
        volunteer.Experience.Value.Should().Be(experience);
        volunteer.Email.Value.Should().Be(email);
        
        volunteer!.TransferDetailsList[0].Name.Should().Be(transferDetails[0].Name);
        volunteer!.TransferDetailsList[0].Description.Should().Be(transferDetails[0].Description);
        volunteer!.TransferDetailsList[1].Name.Should().Be(transferDetails[1].Name);
        volunteer!.TransferDetailsList[1].Description.Should().Be(transferDetails[1].Description);
        volunteer!.TransferDetailsList.Count.Should().Be(2);
        
        // all data is bounded correctly
        volunteer!.SocialNetworksList[0].Name.Should().Be(socialNetworks[0].Name);
        volunteer!.SocialNetworksList[0].Link.Should().Be(socialNetworks[0].Link);
        volunteer!.SocialNetworksList[1].Name.Should().Be(socialNetworks[1].Name);
        volunteer!.SocialNetworksList[1].Link.Should().Be(socialNetworks[1].Link);
        volunteer.SocialNetworksList.Count.Should().Be(2);
    }
}