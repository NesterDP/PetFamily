using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.Commands.UpdateTransferDetails;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class UpdateTransferDetailsHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, UpdateTransferDetailsCommand> _sut;

    public UpdateTransferDetailsHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateTransferDetailsCommand>>();
    }

    [Fact]
    public async Task UpdateSocialNetworks_success_should_replace_existed_social_networks_with_new_ones()
    {
        // arrange
        List<TransferDetailDto> transferDetails =
        [
            new TransferDetailDto("mir", "for transfers within country"),
            new TransferDetailDto("visa", "for transfers outside of country")
        ];

        var volunteer = await DataGenerator.SeedVolunteer(WriteDbContext);

        var command = new UpdateTransferDetailsCommand(
            volunteer.Id,
            transferDetails);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value);

        updatedVolunteer!.TransferDetailsList[0].Name.Should().Be(transferDetails[0].Name);
        updatedVolunteer!.TransferDetailsList[0].Description.Should().Be(transferDetails[0].Description);
        updatedVolunteer!.TransferDetailsList[1].Name.Should().Be(transferDetails[1].Name);
        updatedVolunteer!.TransferDetailsList[1].Description.Should().Be(transferDetails[1].Description);
        updatedVolunteer!.TransferDetailsList.Count.Should().Be(2);
    }
}