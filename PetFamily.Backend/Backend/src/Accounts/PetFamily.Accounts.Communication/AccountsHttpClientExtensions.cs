using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PetFamily.Accounts.Communication;

public static class AccountsHttpClientExtensions
{
    public static IServiceCollection AddAccountsHttpCommunication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AccountsHttpClientOptions>(configuration
            .GetSection(AccountsHttpClientOptions.ACCOUNTS_HTTP_CLIENT));
        
        services.AddHttpClient<IAccountsHttpClient, AccountsHttpClient>((sp, config) =>
        {
            var accountsOptions = sp.GetRequiredService<IOptions<AccountsHttpClientOptions>>().Value;

            config.BaseAddress = new Uri(accountsOptions.Url);
        });

        return services;
    }
}
