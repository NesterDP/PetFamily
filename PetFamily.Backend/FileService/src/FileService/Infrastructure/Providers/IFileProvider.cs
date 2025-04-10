using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Core.CustomErrors;
using UploadPresignedUrlRequest = FileService.Features.UploadPresignedUrl.UploadPresignedUrlRequest;
using UploadPresignedUrlPart = FileService.Features.UploadPresignedUrlPart.UploadPresignedUrlPartRequest;
using StartMultipartUploadRequest = FileService.Features.StartMultipartUpload.StartMultipartUploadRequest;
using CompleteMultipartRequest = FileService.Features.CompleteMultipartUpload.CompleteMultipartRequest;
using StartMultipartUploadResponse = FileService.Features.StartMultipartUpload.StartMultipartUploadResponse;
using UploadResponse = FileService.Features.UploadPresignedUrl.UploadResponse;
using ProviderGetResponse = FileService.Features.GetPresignedUrl.ProviderGetResponse;

namespace FileService.Infrastructure.Providers;

public interface IFileProvider
{
    public Task ConfirmExistence(string key);
    
    public Task<UploadResponse> GenerateUploadUrl(UploadPresignedUrlRequest request);

    public Task<Result<List<ProviderGetResponse>, Error>> GenerateGetUrls(
        List<string> keys,
        CancellationToken cancellationToken);

    public Task<StartMultipartUploadResponse> GenerateStartingMultipartUploadData(
        StartMultipartUploadRequest request,
        CancellationToken cancellationToken);

    public Task<UploadResponse> GenerateUploadUrlPart(
        UploadPresignedUrlPart request,
        string key);

    public Task<GetObjectMetadataResponse> GenerateCompeteMultipartUploadData(
        CompleteMultipartRequest request,
        string key,
        CancellationToken cancellationToken);

    public Task<List<string>> DeleteFiles(
        List<string> keys,
        CancellationToken cancellationToken);
}