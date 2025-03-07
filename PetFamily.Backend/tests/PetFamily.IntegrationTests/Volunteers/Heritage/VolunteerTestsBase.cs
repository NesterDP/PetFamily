using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Database;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.Entities;
using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.IntegrationTests.Volunteers.Heritage;

public class VolunteerTestsBase : IClassFixture<VolunteersTestsWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestsWebFactory Factory;
    protected readonly WriteDbContext WriteDbContext;
    protected readonly IReadDbContext ReadDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;

    protected VolunteerTestsBase(VolunteersTestsWebFactory factory)
    {
        Scope = factory.Services.CreateScope();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        ReadDbContext = Scope.ServiceProvider.GetRequiredService<IReadDbContext>();
        Fixture = new Fixture();
        Factory = factory;
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }

    protected async Task<Guid> SeedVolunteer()
    {
        var volunteerId = VolunteerId.NewVolunteerId();
        var email = Email.Create("test@test.com").Value;
        var fullName = FullName
            .Create("testFirstName", "testLastName", "testSurname").Value;
        var description = Description.Create("testDescription").Value;
        var experience = Experience.Create(1).Value;
        var phoneNumber = Phone.Create("1-2-333-44-55-66").Value;
        var socialNetworksList = new List<SocialNetwork>();
        var transferDetailsList = new List<TransferDetail>();

        var volunteer = new Volunteer(
            volunteerId,
            fullName,
            email,
            description,
            experience,
            phoneNumber,
            socialNetworksList,
            transferDetailsList);

        await WriteDbContext.Volunteers.AddAsync(volunteer);
        await WriteDbContext.SaveChangesAsync();

        return volunteer.Id;
    }

    protected async Task<Guid> SeedSpecies()
    {
        var speciesId = Guid.NewGuid();
        var name = Name.Create("test specie").Value;

        var species = new Species(speciesId, name);

        await WriteDbContext.Species.AddAsync(species);
        await WriteDbContext.SaveChangesAsync();

        return speciesId;
    }

    protected async Task<Guid> SeedBreed(Guid speciesId)
    {
        var breedId = Guid.NewGuid();
        var name = Name.Create("test breed").Value;
        var breed = new Breed(breedId, name);

        var species = await WriteDbContext.Species.FirstOrDefaultAsync(s => s.Id == speciesId);
        species!.AddBreed(breed);
        await WriteDbContext.SaveChangesAsync();

        return breedId;
    }
}