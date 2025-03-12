using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Volunteers.Commands.Create;
using PetFamily.Application.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Domain.VolunteerManagment.ValueObjects.VolunteerVO;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Core.SharedVO;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class UpdateMainInfoHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, UpdateMainInfoCommand> _sut;

    public UpdateMainInfoHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateMainInfoCommand>>();
    }

    [Fact]
    public async Task UpdateVolunteerMainInfo_success_should_update_all_but_socialNetworks_and_transferDetails()
    {
        // arrange
        List<SocialNetwork> socialNetworks =
        [
            SocialNetwork.Create("vk", "vk.com").Value,
            SocialNetwork.Create("mail", "mail.com").Value
        ];
        
        List<TransferDetail> transferDetails =
        [
            TransferDetail.Create("mir", "for transfers within country").Value,
            TransferDetail.Create("visa", "for transfers outside of country").Value
        ];

        var volunteer = await DataGenerator.SeedVolunteer(WriteDbContext);
        volunteer.UpdateSocialNetworks(socialNetworks);
        volunteer.UpdateTransferDetails(transferDetails);
        WriteDbContext.SaveChangesAsync();
        
        const string firstName = "Alexandr";
        const string lastName = "Volkov";
        const string surname = "Alexandrovich";
        const string email = "test@mail.com";
        const string phoneNumber = "1-2-333-44-55-66";
        const string description = "Test description";
        const int experience = 2;
        var fullName = new FullNameDto(firstName, lastName, surname);
        
        
        var command = new UpdateMainInfoCommand(
            volunteer.Id,
            fullName,
            email,
            description,
            experience,
            phoneNumber);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value);

        // all data is updated correctly
        updatedVolunteer.FullName.FirstName.Should().Be(firstName);
        updatedVolunteer.FullName.LastName.Should().Be(lastName);
        updatedVolunteer.FullName.Surname.Should().Be(surname);
        updatedVolunteer.Description.Value.Should().Be(description);
        updatedVolunteer.Experience.Value.Should().Be(experience);
        updatedVolunteer.Email.Value.Should().Be(email);
        
        // unchanged data shouldn't be affected
        updatedVolunteer!.TransferDetailsList[0].Name.Should().Be(transferDetails[0].Name);
        updatedVolunteer!.TransferDetailsList[0].Description.Should().Be(transferDetails[0].Description);
        updatedVolunteer!.TransferDetailsList[1].Name.Should().Be(transferDetails[1].Name);
        updatedVolunteer!.TransferDetailsList[1].Description.Should().Be(transferDetails[1].Description);
        volunteer.TransferDetailsList.Count.Should().Be(2);
        
        updatedVolunteer!.SocialNetworksList[0].Name.Should().Be(socialNetworks[0].Name);
        updatedVolunteer!.SocialNetworksList[0].Link.Should().Be(socialNetworks[0].Link);
        updatedVolunteer!.SocialNetworksList[1].Name.Should().Be(socialNetworks[1].Name);
        updatedVolunteer!.SocialNetworksList[1].Link.Should().Be(socialNetworks[1].Link);
        updatedVolunteer.SocialNetworksList.Count.Should().Be(2);
    }
}