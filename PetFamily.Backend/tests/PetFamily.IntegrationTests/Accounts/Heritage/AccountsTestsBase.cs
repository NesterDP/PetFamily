using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure.DbContexts;

namespace PetFamily.IntegrationTests.Accounts.Heritage;

//public class VolunteerTestsBase : сVolunteerTestsWebFactory>, IAsyncLifetime
public class AccountsTestsBase : IClassFixture<AccountsTestsWebFactory>, IAsyncLifetime
{
    protected readonly AccountsDbContext AccountsDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;
    protected readonly UserManager<User> UserManager;
    protected readonly AccountsTestsWebFactory Factory;

    public AccountsTestsBase(AccountsTestsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        Fixture = new Fixture();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}