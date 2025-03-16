using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain.DataModels;

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
        
        modelBuilder.Entity<Role>()
            .ToTable("roles");
        
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