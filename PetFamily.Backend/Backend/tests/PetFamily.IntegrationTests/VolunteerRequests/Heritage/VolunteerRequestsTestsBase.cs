using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Infrastructure.DbContexts;
using DiscussionsWriteDbContext = PetFamily.Discussions.Infrastructure.DbContexts.WriteDbContext;
using AccountsDbContext = PetFamily.Accounts.Infrastructure.DbContexts.AccountsDbContext;

namespace PetFamily.IntegrationTests.VolunteerRequests.Heritage;

public class VolunteerRequestsTestsBase : IClassFixture<VolunteerRequestsWebFactory>, IAsyncLifetime
{
    protected readonly WriteDbContext WriteDbContext;
    protected readonly DiscussionsWriteDbContext DiscussionsDbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;
    protected readonly VolunteerRequestsWebFactory Factory;
    protected readonly UserManager<User> UserManager;
    protected readonly RoleManager<Role> RoleManager;
    protected readonly AccountsDbContext AccountsDbContext;
    protected readonly IReadDbContext ReadDbContext;

    public VolunteerRequestsTestsBase(VolunteerRequestsWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        DiscussionsDbContext = Scope.ServiceProvider.GetRequiredService<DiscussionsWriteDbContext>();
        WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        ReadDbContext = Scope.ServiceProvider.GetRequiredService<IReadDbContext>();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        AccountsDbContext = Scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        
        Fixture = new Fixture();
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}