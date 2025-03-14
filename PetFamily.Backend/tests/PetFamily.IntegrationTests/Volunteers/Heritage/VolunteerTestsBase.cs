using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure.DbContexts;

namespace PetFamily.IntegrationTests.Volunteers.Heritage;

//public class VolunteerTestsBase : сVolunteerTestsWebFactory>, IAsyncLifetime
public class VolunteerTestsBase : IClassFixture<VolunteerTestsWebFactory>, IAsyncLifetime
{
    protected readonly WriteDbContext VolunteersWriteDbContext;
    protected readonly IReadDbContext VolunteersReadDbContext;
    protected readonly Species.Infrastructure.DbContexts.WriteDbContext SpeciesWriteDbContext;
    protected readonly Species.Application.IReadDbContext SpeciesReadDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;
    protected readonly VolunteerTestsWebFactory Factory;

    public VolunteerTestsBase(VolunteerTestsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        /*VolunteersWriteDbContext = Scope.ServiceProvider.GetRequiredKeyedService<WriteDbContext>("Volunteers");
        VolunteersReadDbContext = Scope.ServiceProvider.GetRequiredKeyedService<IReadDbContext>("Volunteers");
        SpeciesWriteDbContext = Scope.ServiceProvider.GetRequiredKeyedService<Species.Infrastructure.DbContexts.WriteDbContext>("Species");
        SpeciesReadDbContext = Scope.ServiceProvider.GetRequiredKeyedService<Species.Application.IReadDbContext>("Species");*/

        VolunteersWriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        VolunteersReadDbContext = Scope.ServiceProvider.GetRequiredService<IReadDbContext>();
        SpeciesWriteDbContext =
            Scope.ServiceProvider.GetRequiredService<Species.Infrastructure.DbContexts.WriteDbContext>();
        SpeciesReadDbContext =
            Scope.ServiceProvider.GetRequiredService<Species.Application.IReadDbContext>();
        Fixture = new Fixture();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}