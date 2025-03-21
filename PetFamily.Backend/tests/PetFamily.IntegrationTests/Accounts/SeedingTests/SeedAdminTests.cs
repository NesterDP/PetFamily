using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.Login;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Accounts.SeedingTests;


public class SeedAdminTests : AccountsTestsBase
{

    public SeedAdminTests(AccountsTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AdminSeeding_success_should_find_admin_account()
    {
        // arrange
        var EMAIL = "admin@admin.com";
        var USERNAME = "admin";
        
        // act
        var result = await UserManager.FindByEmailAsync(EMAIL);
        var count = UserManager.Users.Count();
        var rolesCount = RoleManager.Roles.Count();

        // assert
        result.Should().NotBeNull();
    }
    
    
    [Fact]
    public async Task AdminSeeding2_success_should_find_admin_account()
    {
        // arrange
        var EMAIL = "admin@admin.com";
        var USERNAME = "admin";
        
        // act
        var result = await UserManager.FindByEmailAsync(EMAIL);
        var count = UserManager.Users.Count();

        // assert
        result.Should().NotBeNull();
    }
}