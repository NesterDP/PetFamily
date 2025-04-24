using Microsoft.EntityFrameworkCore;
using NotificationService.DataModels;

namespace NotificationService.Infrastructure.DbContexts;

public class WriteDbContext : DbContext
{
    private readonly string _connectionString;

    public WriteDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<UserNotificationSettings> UsersNotificationSettings => Set<UserNotificationSettings>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WriteDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Write") ?? false);

        modelBuilder.HasDefaultSchema("NotificationService");
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}