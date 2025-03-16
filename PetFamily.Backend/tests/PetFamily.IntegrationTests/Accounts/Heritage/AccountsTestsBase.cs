using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Infrastructure;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure.DbContexts;

namespace PetFamily.IntegrationTests.Accounts.Heritage;

//public class VolunteerTestsBase : сVolunteerTestsWebFactory>, IAsyncLifetime
public class AccountsTestsBase : IClassFixture<AccountsTestsWebFactory>, IAsyncLifetime
{
    protected readonly AuthorizationDbContext AccountsDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;
    protected readonly AccountsTestsWebFactory Factory;

    public AccountsTestsBase(AccountsTestsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        Fixture = new Fixture();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}