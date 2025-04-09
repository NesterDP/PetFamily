using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Volunteers.Application.Commands.Create;

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
        
        var createVolunteerDto = new CreateVolunteerDto(
            fullName,
            email,
            phoneNumber,
            description,
            experience);
        
        var command = new CreateVolunteerCommand(createVolunteerDto);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value);

        // all data is bounded correctly
        volunteer.FullName.FirstName.Should().Be(firstName);
        volunteer.FullName.LastName.Should().Be(lastName);
        volunteer.FullName.Surname.Should().Be(surname);
        volunteer.Description.Value.Should().Be(description);
        volunteer.Experience.Value.Should().Be(experience);
        volunteer.Email.Value.Should().Be(email);
    }
}