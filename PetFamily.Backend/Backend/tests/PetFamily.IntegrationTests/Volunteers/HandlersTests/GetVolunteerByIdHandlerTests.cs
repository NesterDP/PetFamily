using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Volunteers.Application.Queries.GetVolunteerById;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class GetVolunteerByIdHandlerTests : VolunteerTestsBase
{
    private readonly IQueryHandler<VolunteerDto, GetVolunteerByIdQuery> _sut;

    public GetVolunteerByIdHandlerTests(VolunteerTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<VolunteerDto, GetVolunteerByIdQuery>>();
    }

    [Fact]
    public async Task GetVolunteerById_returns_info_about_volunteer()
    {
        // arrange
        var volunteer = await DataGenerator.SeedVolunteer(VolunteersWriteDbContext);
        await VolunteersWriteDbContext.SaveChangesAsync();
        await DataGenerator.SeedVolunteer(VolunteersWriteDbContext);
        var query = new GetVolunteerByIdQuery(volunteer.Id);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(volunteer.Id);
        result.FirstName.Should().Be(volunteer.FullName.FirstName);
        result.LastName.Should().Be(volunteer.FullName.LastName);
        result.Surname.Should().Be(volunteer.FullName.Surname);
        result.PhoneNumber.Should().Be(volunteer.PhoneNumber.Value);
        result.Email.Should().Be(volunteer.Email.Value);
        result.Description.Should().Be(volunteer.Description.Value);
        result.Experience.Should().Be(volunteer.Experience.Value);
        result.IsDeleted.Should().Be(volunteer.IsDeleted);
    }

    [Fact]
    public async Task GetPetById_returns_null_for_soft_deleted_volunteer()
    {
        // arrange
        const int PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(
            VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        volunteer.Delete();
        await VolunteersWriteDbContext.SaveChangesAsync();
        var query = new GetVolunteerByIdQuery(volunteer.Id);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPetById_returns_null_for_volunteer_that_does_not_exist()
    {
        // arrange
        const int PET_COUNT = 5;
        await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var query = new GetVolunteerByIdQuery(Guid.NewGuid());

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Should().BeNull();
    }
}