using Microsoft.AspNetCore.Builder;

namespace PetFamily.Core.Extensions;

public static class ApplicationExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        /*await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await dbContext.Database.MigrateAsync();*/
    }
}