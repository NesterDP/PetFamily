using Microsoft.EntityFrameworkCore;
using Npgsql;
using PetFamily.Accounts.Infrastructure.DbContexts;

namespace PetFamily.Web.ApplicationConfiguration;

public static class MigrationsApplier
{
    public static void ApplyMigration<TDbContext>(IServiceProvider serviceProvider)
        where TDbContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        // Проверяем, есть ли pending миграции
        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

        if (pendingMigrations.Count == 0)
        {
            return;
        }

        try
        {
            // Применяем миграции
            dbContext.Database.Migrate();
        }
        catch (Exception ex) when (IsMigrationConflictException(ex))
        {
        }
    }

    public static void ApplyMigrations(this IApplicationBuilder builder)
    {
        var serviceProvider = builder.ApplicationServices;

        ApplyMigration<Volunteers.Infrastructure.DbContexts.WriteDbContext>(serviceProvider);

        ApplyMigration<Species.Infrastructure.DbContexts.WriteDbContext>(serviceProvider);

        ApplyMigration<AccountsDbContext>(serviceProvider);

        ApplyMigration<VolunteerRequests.Infrastructure.DbContexts.WriteDbContext>(serviceProvider);

        ApplyMigration<Discussions.Infrastructure.DbContexts.WriteDbContext>(serviceProvider);
    }

    private static bool IsMigrationConflictException(Exception ex)
    {
        return ex switch
        {
            PostgresException pgEx => pgEx.SqlState == "42P07", // PostgreSQL
            _ => ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)
        };
    }
}