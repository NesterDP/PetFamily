using CSharpFunctionalExtensions;
using PetFamily.Application.FilesManagement.FilesData;
using PetFamily.Application.FilesManagement.Upload;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Providers;

public interface IFilesProvider
{
    public Task<Result<string, Error>> UploadFile(
        UploadData uploadData, CancellationToken cancellationToken = default);
    
    public Task<Result<string, Error>> DeleteFile(
        DeleteData deleteData, CancellationToken cancellationToken = default);
    
    public Task<Result<string, Error>> GetPresignedUrl(
        GetPresignedUrlData getPresignedUrlData, CancellationToken cancellationToken = default);
}