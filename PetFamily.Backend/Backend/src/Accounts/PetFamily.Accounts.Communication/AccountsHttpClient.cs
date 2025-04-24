using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CSharpFunctionalExtensions;
using PetFamily.Accounts.Contracts.Dto;
using PetFamily.Accounts.Contracts.Requests;

namespace PetFamily.Accounts.Communication;

public class AccountsHttpClient : IAccountsService
{
    private readonly HttpClient _httpClient;

    public AccountsHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<UserInfoDto, string>> GetUserInfoById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"api/Accounts/{userId}", cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Failed to get user info";

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(responseJson);

        if (!doc.RootElement.TryGetProperty("result", out var resultElement))
            return "Response format invalid: missing 'result' property";

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var userInfo = JsonSerializer.Deserialize<UserInfoDto>(resultElement.GetRawText(), options);
        if (userInfo == null)
            return "Failed to parse UserInfoDto";

        return userInfo!;
    }

    public async Task<Result<string, string>> GenerateEmailConfirmationToken(
        GenerateEmailTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient
            .PostAsJsonAsync("api/Accounts/email-token-generation", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return Result.Failure<string, string>("Failed to get email confirmation token");

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(responseJson);

        if (!doc.RootElement.TryGetProperty("result", out var resultElement))
            return Result.Failure<string, string>("Response format invalid: missing 'result' property");

        var confirmationToken = JsonSerializer.Deserialize<string>(resultElement.GetRawText());
        if (confirmationToken == null)
            return Result.Failure<string, string>("Failed to parse token");

        return Result.Success<string, string>(confirmationToken);
    }
}