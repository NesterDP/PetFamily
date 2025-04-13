using FileService.Communication;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Contracts.SubModels;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace PetFamily.IntegrationTests.Accounts.Heritage;

public class AccountsTestsWebFactory : IntegrationTestsWebFactory
{
    private readonly IFileService _fileServiceMock = Substitute.For<IFileService>();

    protected override void ConfigureDefaultServices(IServiceCollection services)
    {
        base.ConfigureDefaultServices(services);

        var fileServiceDescriptor = services.SingleOrDefault(s =>
            s.ServiceType == typeof(IFileService));

        if (fileServiceDescriptor is not null)
            services.Remove(fileServiceDescriptor);

        services.AddTransient<IFileService>(_ => _fileServiceMock);
    }

    public void SetupSuccessStartMultipartMock(int filesAmount = 1)
    {
        var response = new StartMultipartUploadResponse([]);

        for (var i = 0; i < filesAmount; i++)
        {
            var startMultipartProviderInfo = new MultipartStartProviderInfo("key" + i, "uploadId" + i);

            response.MultipartStartInfos.Add(startMultipartProviderInfo);
        }

        _fileServiceMock
            .StartMultipartUpload(Arg.Any<StartMultipartUploadRequest>(), Arg.Any<CancellationToken>())
            .Returns(response);
    }

    public void SetupFailureStartMultipartMock()
    {
        _fileServiceMock
            .StartMultipartUpload(Arg.Any<StartMultipartUploadRequest>(), Arg.Any<CancellationToken>())
            .Returns("error");
    }

    public void SetupSuccessCompleteMultipartMock(List<Guid> fileIds)
    {
        var response = new CompleteMultipartUploadResponse([]);

        for (var i = 0; i < fileIds.Count; i++)
        {
            var completeMultipartFileInfo = new MultipartCompleteFileInfo(
                fileIds[i],
                "key" + i,
                "image/jpg",
                123);

            response.MultipartCompleteInfos.Add(completeMultipartFileInfo);
        }

        _fileServiceMock
            .CompleteMultipartUpload(Arg.Any<CompleteMultipartUploadRequest>(), Arg.Any<CancellationToken>())
            .Returns(response);
    }

    public void SetupFailureCompleteMultipartMock()
    {
        _fileServiceMock
            .CompleteMultipartUpload(Arg.Any<CompleteMultipartUploadRequest>(), Arg.Any<CancellationToken>())
            .Returns("error");
    }

    public void SetupSuccessGetFilesPresignedUrlsMock(List<Guid> fileIds)
    {
        var response = new GetFilesPresignedUrlsResponse([]);

        for (var i = 0; i < fileIds.Count; i++)
        {
            var extendedFileInfo = new ExtendedFileInfo(
                fileIds[i],
                "key" + i,
                "url" + i,
                DateTime.UtcNow,
                123,
                "image/jpg");

            response.FilesInfos.Add(extendedFileInfo);
        }

        _fileServiceMock
            .GetFilesPresignedUrls(Arg.Any<GetFilesPresignedUrlsRequest>(), Arg.Any<CancellationToken>())
            .Returns(response);
    }

    public void SetupFailureGetFilesPresignedUrlsMock()
    {
        _fileServiceMock
            .GetFilesPresignedUrls(Arg.Any<GetFilesPresignedUrlsRequest>(), Arg.Any<CancellationToken>())
            .Returns("error");
    }
    
    public void SetupSuccessDeleteFilesByIdsMock(List<Guid> fileIds)
    {
        var response = new DeleteFilesByIdsResponse([]);

        for (var i = 0; i < fileIds.Count; i++)
        {
            var key = "key" + i;

            response.Keys.Add(key);
        }

        _fileServiceMock
            .DeleteFilesByIds(Arg.Any<DeleteFilesByIdsRequest>(), Arg.Any<CancellationToken>())
            .Returns(response);
    }

    public void SetUpFailureDeleteFilesByIdsMock()
    {
        _fileServiceMock
            .DeleteFilesByIds(Arg.Any<DeleteFilesByIdsRequest>(), Arg.Any<CancellationToken>())
            .Returns("error");
    }
}