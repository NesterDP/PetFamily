using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Volunteers.Application.Commands.DeletePet;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class SoftDeletePetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, DeletePetCommand> _sut;

    public SoftDeletePetHandlerTests(VolunteerTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<SoftDeletePetHandler>();
    }

    [Fact]
    public async Task SoftDeletePet_success_should_soft_delete_pet()
    {
        // arrange
        const int PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(
            VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        var command = new DeletePetCommand(volunteer.Id, pet.Id);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value)!.IsDeleted.Should().BeTrue();
    }
}