using PetFamily.Accounts.Infrastructure.Seeding;

namespace PetFamily.Web.ApplicationConfiguration;

public static class SeedDataApplier
{
    public static async Task SeedAsync(this IApplicationBuilder builder)
    {
        var accountsSeeder = builder.ApplicationServices.GetRequiredService<AccountsSeeder>();
        await accountsSeeder.SeedAsync();
    }
}