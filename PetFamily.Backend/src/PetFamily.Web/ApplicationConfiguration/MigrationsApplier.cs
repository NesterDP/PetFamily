using Microsoft.EntityFrameworkCore;
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
        if (pendingMigrations.Any())
        {
            // Применяем миграции
            dbContext.Database.Migrate();
        }
    }

    public static void ApplyMigrations(this IApplicationBuilder builder)
    {
        var serviceProvider = builder.ApplicationServices;
        
        ApplyMigration<PetFamily.Volunteers.Infrastructure.DbContexts.WriteDbContext>(serviceProvider);
        
        ApplyMigration<PetFamily.Species.Infrastructure.DbContexts.WriteDbContext>(serviceProvider);
        
        ApplyMigration<AccountsDbContext>(serviceProvider);
        
        ApplyMigration<PetFamily.VolunteerRequests.Infrastructure.DbContexts.WriteDbContext>(serviceProvider);
    }
}