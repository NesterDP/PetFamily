using CSharpFunctionalExtensions;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.FilesManagement.GetPresignedUrl;

public class GetPresignedUrlHandler
{
    private readonly IFilesProvider _filesProvider;

    public GetPresignedUrlHandler(IFilesProvider filesProvider)
    {
        _filesProvider = filesProvider;
    }

    public async Task<Result<string, Error>> HandleAsync(
        GetPresignedUrlRequest getPresignedUrlRequest,
        CancellationToken cancellationToken = default)
    {
        return await _filesProvider.GetPresignedUrl(getPresignedUrlRequest.GetPresignedUrlData, cancellationToken);
    }
}