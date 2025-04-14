using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;

namespace FileService.Communication;

public interface IFileService
{
    Task<Result<GetFilesPresignedUrlsResponse, string>> GetFilesPresignedUrls(
        GetFilesPresignedUrlsRequest request,
        CancellationToken cancellationToken);

    Task<Result<StartMultipartUploadResponse, string>> StartMultipartUpload(
        StartMultipartUploadRequest request,
        CancellationToken cancellationToken);

    Task<Result<CompleteMultipartUploadResponse, string>> CompleteMultipartUpload(
        CompleteMultipartUploadRequest request,
        CancellationToken cancellationToken);

    Task<Result<DeleteFilesByIdsResponse, string>> DeleteFilesByIds(
        DeleteFilesByIdsRequest request,
        CancellationToken cancellationToken);
}