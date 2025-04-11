using System.Net;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;

namespace FileService.Communication;

public class FileHttpClient
{
    private readonly HttpClient _httpClient;

    public FileHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<IReadOnlyList<GetFilesPresignedUrlsResponse>, string>> GetFilesPresignedUrls(
        GetFilesPresignedUrlsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("files/presigned-urls", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Failed to get files presigned urls";

        var fileResponse = await response.Content
            .ReadFromJsonAsync<IReadOnlyList<GetFilesPresignedUrlsResponse>>(cancellationToken);

        return fileResponse?.ToList() ?? [];
    }
}