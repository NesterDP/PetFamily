namespace PetFamily.Accounts.Communication;

public class AccountsHttpClientOptions
{
    public const string ACCOUNTS_HTTP_CLIENT = "AccountsHttpClient";
    
    public string Url { get; init; } = string.Empty;
}