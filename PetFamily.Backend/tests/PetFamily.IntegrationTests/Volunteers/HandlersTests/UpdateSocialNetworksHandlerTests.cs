using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Application.Volunteers.Commands.UpdatePetMainPhoto;
using PetFamily.Application.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class UpdateSocialNetworksHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, UpdateSocialNetworksCommand> _sut;

    public UpdateSocialNetworksHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateSocialNetworksCommand>>();
    }

    [Fact]
    public async Task UpdateSocialNetworks_success_should_replace_existed_social_networks_with_new_ones()
    {
        // arrange
        List<SocialNetworkDto> socialNetworks =
        [
            new SocialNetworkDto("vk", "vk.com"),
            new SocialNetworkDto("mail", "mail.com")
        ];

        var volunteer = await DataGenerator.SeedVolunteer(WriteDbContext);


        var command = new UpdateSocialNetworksCommand(
            volunteer.Id,
            socialNetworks);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value);

        updatedVolunteer!.SocialNetworksList[0].Name.Should().Be(socialNetworks[0].Name);
        updatedVolunteer!.SocialNetworksList[0].Link.Should().Be(socialNetworks[0].Link);
        updatedVolunteer!.SocialNetworksList[1].Name.Should().Be(socialNetworks[1].Name);
        updatedVolunteer!.SocialNetworksList[1].Link.Should().Be(socialNetworks[1].Link);
        updatedVolunteer.SocialNetworksList.Count.Should().Be(2);
    }
}