using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Accounts.Infrastructure.EntityManagers;
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
    protected readonly RoleManager<Role> RoleManager;
    protected readonly IAccountManager AccountManager;
    protected readonly AccountsTestsWebFactory Factory;
    protected readonly ITokenProvider TokenProvider;

    public AccountsTestsBase(AccountsTestsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        AccountsDbContext = Scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        TokenProvider = Scope.ServiceProvider.GetRequiredService<ITokenProvider>();
        AccountManager = Scope.ServiceProvider.GetRequiredService<IAccountManager>();
        Fixture = new Fixture();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}