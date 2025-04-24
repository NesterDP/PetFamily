using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PetFamily.Accounts.Communication;

public static class AccountsServiceExtensions
{
    public static IServiceCollection AddAccountsService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AccountsServiceOptions>(configuration
            .GetSection(AccountsServiceOptions.ACCOUNTS_SERVICE));

        services.AddHttpClient<IAccountsService, AccountsHttpClient>((sp, config) =>
        {
            var accountsOptions = sp.GetRequiredService<IOptions<AccountsServiceOptions>>().Value;

            config.BaseAddress = new Uri(accountsOptions.Url);
        });

        return services;
    }
}
