using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Contracts.SubModels;
using FileService.Core.CustomErrors;
using CompleteMultipartUploadRequest = FileService.Contracts.Requests.CompleteMultipartUploadRequest;

namespace FileService.Infrastructure.Providers;

public interface IFilesProvider
{
    public Task ConfirmExistence(string key);
    
    public Task<MinimalFileInfo> GenerateUploadUrl(UploadPresignedUrlRequest request);

    public Task<Result<List<MinimalFileInfo>, Error>> GenerateGetUrls(
        List<string> keys,
        CancellationToken cancellationToken);

    public Task<MultipartStartInfo> GenerateStartingMultipartUploadData(
        StartMultipartUploadRequest request,
        CancellationToken cancellationToken);

    public Task<MinimalFileInfo> GenerateUploadUrlPart(
        UploadPresignedUrlPartRequest request,
        string key);

    public Task<GetObjectMetadataResponse> GenerateCompeteMultipartUploadData(
        CompleteMultipartUploadRequest uploadRequest,
        string key,
        CancellationToken cancellationToken);

    public Task<List<string>> DeleteFiles(
        List<string> keys,
        CancellationToken cancellationToken);
}