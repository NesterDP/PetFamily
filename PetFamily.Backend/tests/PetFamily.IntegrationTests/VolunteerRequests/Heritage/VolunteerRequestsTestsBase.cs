using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.VolunteerRequests.Infrastructure.DbContexts;

namespace PetFamily.IntegrationTests.VolunteerRequests.Heritage;

public class VolunteerRequestsTestsBase : IClassFixture<VolunteerRequestsWebFactory>, IAsyncLifetime
{
    protected readonly WriteDbContext WriteDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;
    protected readonly VolunteerRequestsWebFactory Factory;

    public VolunteerRequestsTestsBase(VolunteerRequestsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        Fixture = new Fixture();
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}