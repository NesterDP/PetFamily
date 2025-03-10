using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Volunteers.Commands.AddPet;
using PetFamily.Application.Volunteers.Commands.DeletePet;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class SoftDeletePetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, DeletePetCommand> _sut;
    public SoftDeletePetHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<SoftDeletePetHandler>();
    }
    
    [Fact]
    public async Task SoftDeletePet_success_should_soft_delete_pet()
    {
        // arrange
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        var command = new DeletePetCommand(volunteer.Id, pet.Id);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync(v => v.Id == volunteer.Id);
        volunteer!.AllOwnedPets.FirstOrDefault(p => p.Id == result.Value)!._isDeleted.Should().BeTrue();
    }
}