using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Species.Commands.Create;
using PetFamily.Application.Species.Commands.DeleteSpeciesById;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.Entities;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.SpeciesNS.Heritage;

namespace PetFamily.IntegrationTests.SpeciesNS.HandlerTests;

public class CreateSpeciesHandlerTests : SpeciesTestsBase
{
    private readonly ICommandHandler<Guid, CreateSpeciesCommand> _sut;

    public CreateSpeciesHandlerTests(SpeciesTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateSpeciesCommand>>();
    }

    [Fact]
    public async Task CreateSpecies_success_should_return_guid_of_created_species()
    {
        // arrange
        var SPECIES_NAME = "Test Specie";
        var command = new CreateSpeciesCommand(SPECIES_NAME);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var species = await WriteDbContext.Species.FirstOrDefaultAsync(v => v.Id == result.Value);
        species.Should().NotBeNull();
        species.Name.Value.Should().Be(SPECIES_NAME);
    }
}