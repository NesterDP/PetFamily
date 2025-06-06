using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.Outbox;

namespace PetFamily.Accounts.Infrastructure.DbContexts;

public class AccountsDbContext
    : IdentityDbContext<User, Role, Guid>
{
    private readonly string _connectionString;

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<AdminAccount> AdminAccounts => Set<AdminAccount>();

    public DbSet<ParticipantAccount> ParticipantAccounts => Set<ParticipantAccount>();

    public DbSet<VolunteerAccount> VolunteerAccounts => Set<VolunteerAccount>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public AccountsDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();

        // optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountsDbContext).Assembly);

        modelBuilder.HasDefaultSchema("accounts");
    }

    // ReSharper disable once UnusedMember.Local
    private static ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}