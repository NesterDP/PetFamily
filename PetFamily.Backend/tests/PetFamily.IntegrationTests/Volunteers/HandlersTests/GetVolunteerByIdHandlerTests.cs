using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Pets.Queries.GetPetById;
using PetFamily.Application.Volunteers.Queries.GetVolunteerById;
using PetFamily.Domain.VolunteerManagment.ValueObjects.VolunteerVO;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Core.SharedVO;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class GetVolunteerByIdHandlerTests : VolunteerTestsBase
{
    private readonly IQueryHandler<VolunteerDto, GetVolunteerByIdQuery> _sut;

    public GetVolunteerByIdHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<VolunteerDto, GetVolunteerByIdQuery>>();
    }

    [Fact]
    public async Task GetVolunteerById_returns_info_about_volunteer()
    {
        var PET_COUNT = 5;
        // arrange
        List<TransferDetail> transferDetails =
        [
            TransferDetail.Create("mir", "for transfers within country").Value,
            TransferDetail.Create("visa", "for transfers outside of country").Value
        ];
        
        List<SocialNetwork> socialNetworks =
        [
            SocialNetwork.Create("vk.com", "my most used social network").Value,
            SocialNetwork.Create("mail.ru", "my less used social network").Value
        ];
        
        var volunteer = await DataGenerator.SeedVolunteer(WriteDbContext);
        volunteer.UpdateSocialNetworks(socialNetworks);
        volunteer.UpdateTransferDetails(transferDetails);
        await WriteDbContext.SaveChangesAsync();
        var anotherVolunteer = await DataGenerator.SeedVolunteer(WriteDbContext);
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
        result.IsDeleted.Should().Be(volunteer._isDeleted);
        
        result.SocialNetworks[0].Name.Should().Be(socialNetworks[0].Name);
        result.SocialNetworks[0].Link.Should().Be(socialNetworks[0].Link);
        result.SocialNetworks[1].Name.Should().Be(socialNetworks[1].Name);
        result.SocialNetworks[1].Link.Should().Be(socialNetworks[1].Link);
        
        result.TransferDetails[0].Name.Should().Be(transferDetails[0].Name);
        result.TransferDetails[0].Description.Should().Be(transferDetails[0].Description);
        result.TransferDetails[1].Name.Should().Be(transferDetails[1].Name);
        result.TransferDetails[1].Description.Should().Be(transferDetails[1].Description);
    }
    
    [Fact]
    public async Task GetPetById_returns_null_for_soft_deleted_volunteer()
    {
        // arrange
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        volunteer.Delete();
        await WriteDbContext.SaveChangesAsync();
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
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var query = new GetVolunteerByIdQuery(Guid.NewGuid());

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.Should().BeNull();
    }
}