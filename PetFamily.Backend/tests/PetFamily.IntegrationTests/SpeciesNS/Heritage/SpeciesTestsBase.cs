using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Database;
using PetFamily.Infrastructure.DbContexts;
using PetFamily.IntegrationTests.Volunteers.Heritage;

namespace PetFamily.IntegrationTests.SpeciesNS.Heritage;
public class SpeciesTestsBase : IClassFixture<SpeciesTestsWebFactory>, IAsyncLifetime
{
    protected readonly WriteDbContext WriteDbContext;
    protected readonly IReadDbContext ReadDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;
    protected readonly SpeciesTestsWebFactory Factory;

    public SpeciesTestsBase(SpeciesTestsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        ReadDbContext = Scope.ServiceProvider.GetRequiredService<IReadDbContext>();
        Fixture = new Fixture();
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}