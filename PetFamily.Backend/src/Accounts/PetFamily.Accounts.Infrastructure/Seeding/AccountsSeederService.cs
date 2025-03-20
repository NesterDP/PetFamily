using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.EntityManagers;
using PetFamily.Accounts.Infrastructure.EntityManagers.ManagersOptions;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Options;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using ApplicationException = System.ApplicationException;

namespace PetFamily.Accounts.Infrastructure.Seeding;

public class AccountsSeederService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly PermissionManager _permissionManager;
    private readonly RolePermissionManager _rolePermissionManager;
    private readonly AdminAccountManager _adminAccountManager;
    private readonly ILogger<AccountsSeederService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AdminOptions _adminOptions;

    public AccountsSeederService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        PermissionManager permissionManager,
        RolePermissionManager rolePermissionManager,
        AdminAccountManager adminAccountManager,
        IOptions<AdminOptions> adminOptions,
        ILogger<AccountsSeederService> logger,
        [FromKeyedServices(UnitOfWorkSelector.Accounts)] IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _permissionManager = permissionManager;
        _rolePermissionManager = rolePermissionManager;
        _adminAccountManager = adminAccountManager;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _adminOptions = adminOptions.Value;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Seeding accounts...");

        var json = await File.ReadAllTextAsync(FilePaths.Accounts);

        var seedData = JsonSerializer.Deserialize<RolePermissionOptions>(json)
                       ?? throw new ApplicationException("Could not deserialize role permission config");

        await SeedPermissions(seedData);

        await SeedRoles(seedData);

        await SeedRolePermissions(seedData);
        
        await SeedAdmin();
    }

    private async Task SeedAdmin()
    {
        var AdminFullName = FullName.Create("AdminFirstName", "AdminLastName", "AdminSurname").Value;
        
        var adminRole = await _roleManager.FindByNameAsync(AdminAccount.ADMIN)
                        ?? throw new ApplicationException("Could not find admin role");
        
        var adminUser = User.CreateAdmin(
            _adminOptions.UserName,
            _adminOptions.Email,
            AdminFullName,
            adminRole);
        
        if (adminUser.IsFailure)
            throw new ApplicationException("wasn't able to create admin instance");
        
        var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var result = await _userManager.CreateAsync(adminUser.Value, _adminOptions.Password);

            if (result.Succeeded)
            {
                var adminAccount = new AdminAccount(adminUser.Value);
        
                await _adminAccountManager.CreateAdminAccount(adminAccount);
            }
        
            transaction.Commit();
            _logger.LogInformation("Successfully seeded admin");
        }
        catch (Exception e)
        {
           transaction.Rollback();
           _logger.LogError(e, "Failed to seed admin");
           throw;
        }
    }

    private async Task SeedRolePermissions(
        RolePermissionOptions seedData)
    {
        foreach (var roleName in seedData.Roles.Keys)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            var rolePermissions = seedData.Roles[roleName];

            await _rolePermissionManager.AddRangeIfNotExist(role!.Id, rolePermissions);
        }

        _logger.LogInformation("Added permissions for roles to database");
    }

    private async Task SeedRoles(RolePermissionOptions seedData)
    {
        foreach (var roleName in seedData.Roles.Keys)
        {
            var existingRole = await _roleManager.FindByNameAsync(roleName);

            if (existingRole is null)
                await _roleManager.CreateAsync(new Role() { Name = roleName });
        }

        _logger.LogInformation("Roles were added to database");
    }

    private async Task SeedPermissions(RolePermissionOptions seedData)
    {
        var permissionsToAdd = seedData.Permissions
            .SelectMany(permissionGroup => permissionGroup.Value);

        await _permissionManager.AddRangeIfNotExist(permissionsToAdd);

        _logger.LogInformation("Permission were added to database");
    }
}