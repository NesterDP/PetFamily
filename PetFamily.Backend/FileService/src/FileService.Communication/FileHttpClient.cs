using System.Net;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;

namespace FileService.Communication;

public class FileHttpClient : IFileService
{
    private readonly HttpClient _httpClient;

    public FileHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Result<GetFilesPresignedUrlsResponse, string>> GetFilesPresignedUrls(
        GetFilesPresignedUrlsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("files/presigned-urls", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Failed to get files presigned urls";

        var fileResponse = await response.Content
            .ReadFromJsonAsync<GetFilesPresignedUrlsResponse>(cancellationToken);

        return fileResponse!;
    }
    
    public async Task<Result<StartMultipartUploadResponse, string>> StartMultipartUpload(
        StartMultipartUploadRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("files/start-multipart", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Failed to start multipart upload";

        var fileResponse = await response.Content
            .ReadFromJsonAsync<StartMultipartUploadResponse>(cancellationToken);

        return fileResponse!;
    }
    
    public async Task<Result<CompleteMultipartUploadResponse, string>> CompleteMultipartUpload(
        CompleteMultipartUploadRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("files/complete-multipart", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Failed to complete multipart upload";

        var fileResponse = await response.Content
            .ReadFromJsonAsync<CompleteMultipartUploadResponse>(cancellationToken);
        
        if (fileResponse!.MultipartCompleteInfos.Count == 0)
            return "Failed to complete multipart upload";

        return fileResponse!;
    }
    
    public async Task<Result<DeleteFilesByIdsResponse, string>> DeleteFilesByIds(
        DeleteFilesByIdsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("files/deletion", request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
            return "Failed to delete files";

        var fileResponse = await response.Content
            .ReadFromJsonAsync<DeleteFilesByIdsResponse>(cancellationToken);

        return fileResponse!;
    }
}