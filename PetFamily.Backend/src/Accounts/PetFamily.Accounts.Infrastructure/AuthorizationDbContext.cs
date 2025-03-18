using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.Constants;

namespace PetFamily.Accounts.Infrastructure;

public class AuthorizationDbContext
    : IdentityDbContext<User, Role, Guid>
{
    private readonly string _connectionString;

    public AuthorizationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        //optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .ToTable("users");
        
        modelBuilder.Entity<User>()
            .Property(v => v.SocialNetworks)
            .CustomListJsonCollectionConverter(
                socialNetwork => new SocialNetworkDto(socialNetwork.Name, socialNetwork.Link),
                dto => new SocialNetworkDto(dto.Link, dto.Name))
            .HasColumnName("social_networks");
        
        modelBuilder.Entity<Role>()
            .ToTable("roles");
        
        modelBuilder.Entity<Permission>()
            .ToTable("permissions");
        
        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Permission>()
            .Property(p => p.Description)
            .HasMaxLength(DomainConstants.MAX_MEDIUM_TEXT_LENGTH);
        
        modelBuilder.Entity<RolePermission>()
            .ToTable("role_permissions");

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);
        
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);
        
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });
        
        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("user_claims");
        
        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("user_tokens");
        
        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("user_logins");
        
        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("role_claims");
        
        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("user_roles");
        
        
        
        
        /*modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AuthorizationDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Write") ?? false);*/
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}