using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Infrastructure.EntityManagers.ManagersOptions;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.SharedKernel.Constants;

namespace PetFamily.IntegrationTests.Accounts.SeedingTests;


public class AccountsSeedingTests : AccountsTestsBase
{

    public AccountsSeedingTests(AccountsTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AdminSeeding_success_should_find_admin_account()
    {
        // arrange
        string? EMAIL = "admin@admin.com";
        string? USERNAME = "admin";
        
        // act
        var result = await UserManager.FindByEmailAsync(EMAIL);

        // assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task PermissionsSeeding_success_permissions_from_database_and_file_should_be_the_same()
    {
        // arrange
        string? json = await File.ReadAllTextAsync(FilePaths.ACCOUNTS);
        var seedData = JsonSerializer.Deserialize<RolePermissionOptions>(json)
                       ?? throw new ApplicationException("Could not deserialize role permission config");
        var permissions = seedData.Permissions
            .SelectMany(x => x.Value)
            .ToList();
        
        // act
        var result = await AccountsDbContext.Permissions.Select(p => p.Code).ToListAsync();

        // assert
        result.Count.Should().Be(permissions.Count);
        result.All(p => permissions.Contains(p)).Should().BeTrue();
    }
    
    [Fact]
    public async Task RolesSeeding_success_roles_from_database_and_file_should_be_the_same()
    {
        // arrange
        string? json = await File.ReadAllTextAsync(FilePaths.ACCOUNTS);
        var seedData = JsonSerializer.Deserialize<RolePermissionOptions>(json)
                       ?? throw new ApplicationException("Could not deserialize role permission config");
        var roles = seedData.Roles.Keys.ToList();
        
        // act
        var result = await AccountsDbContext.Roles.Select(r => r.Name).ToListAsync();

        // assert
        result.Count.Should().Be(roles.Count);
        result.All(r => roles.Contains(r)).Should().BeTrue();
    }
}