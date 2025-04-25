using Microsoft.Extensions.DependencyInjection;
using PetFamily.Discussions.Infrastructure.DbContexts;

namespace PetFamily.IntegrationTests.Discussions.Heritage;

public class DiscussionsTestsBase : IClassFixture<DiscussionsWebFactory>, IAsyncLifetime
{
    protected readonly WriteDbContext WriteDbContext;
    protected readonly IServiceScope Scope;
    protected readonly DiscussionsWebFactory Factory;

    public DiscussionsTestsBase(DiscussionsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}