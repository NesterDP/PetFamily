using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Volunteers.Commands.Delete;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class HardDeleteVolunteerHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, DeleteVolunteerCommand> _sut;
    public HardDeleteVolunteerHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<HardDeleteVolunteerHandler>();
    }
    
    [Fact]
    public async Task SoftDeleteVolunteer_success_should_hard_delete_volunteer_and_all_his_pets()
    {
        // arrange
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var command = new DeleteVolunteerCommand(volunteer.Id);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var deletedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        
        // volunteer and all his pets should be deleted from database
        deletedVolunteer.Should().BeNull();
        foreach (var pet in volunteer.AllOwnedPets)
        {
            var deletedPet = await ReadDbContext.Pets.FirstOrDefaultAsync(p => p.Id == pet.Id);
            deletedPet.Should().BeNull();
        }
    }
}