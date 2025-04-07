using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Volunteers.Application.Commands.Delete;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;


public class SoftDeleteVolunteerHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, DeleteVolunteerCommand> _sut;
    public SoftDeleteVolunteerHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<SoftDeleteVolunteerHandler>();
    }
    
    [Fact]
    public async Task SoftDeleteVolunteer_success_should_soft_delete_volunteer_and_all_his_pets()
    {
        // arrange
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var command = new DeleteVolunteerCommand(volunteer.Id);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        volunteer.IsDeleted.Should().BeTrue();
        volunteer!.AllOwnedPets.All(p => p.IsDeleted == true)!.Should().BeTrue();
    }
}