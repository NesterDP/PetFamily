using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Volunteers.Application.Commands.UpdateMainInfo;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class UpdateMainInfoHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, UpdateMainInfoCommand> _sut;

    public UpdateMainInfoHandlerTests(VolunteerTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateMainInfoCommand>>();
    }

    [Fact]
    public async Task UpdateVolunteerMainInfo_success_should_update_all_but_socialNetworks_and_transferDetails()
    {
        // arrange
        var volunteer = await DataGenerator.SeedVolunteer(VolunteersWriteDbContext);
        await VolunteersWriteDbContext.SaveChangesAsync();

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

        var updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value);

        // all data is updated correctly
        updatedVolunteer!.FullName.FirstName.Should().Be(firstName);
        updatedVolunteer.FullName.LastName.Should().Be(lastName);
        updatedVolunteer.FullName.Surname.Should().Be(surname);
        updatedVolunteer.Description.Value.Should().Be(description);
        updatedVolunteer.Experience.Value.Should().Be(experience);
        updatedVolunteer.Email.Value.Should().Be(email);
    }
}