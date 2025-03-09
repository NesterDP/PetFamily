using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using PetFamily.Application.Files;
using PetFamily.Application.Files.FilesData;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using FileInfo = PetFamily.Application.Files.FilesData.FileInfo;

namespace PetFamily.IntegrationTests.Volunteers.Heritage;

public class VolunteerTestsWebFactory : IntegrationTestsWebFactory
{
    private readonly IFilesProvider _filesServiceMock = Substitute.For<IFilesProvider>();

    protected override void ConfigureDefaultServices(IServiceCollection services)
    {
        base.ConfigureDefaultServices(services);

        var fileServiceDescriptor = services.SingleOrDefault(s =>
            s.ServiceType == typeof(IFilesProvider));

        if (fileServiceDescriptor is not null)
            services.Remove(fileServiceDescriptor);

        services.AddTransient<IFilesProvider>(_ => _filesServiceMock);
    }

    public void SetupSuccessFileServiceMock()
    {
        List<FilePath> data = [FilePath.Create("first_file").Value];
        IReadOnlyList<FilePath> response = data;

        _filesServiceMock
            .UploadFiles(Arg.Any<IEnumerable<FileData>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IReadOnlyList<FilePath>, Error>(data));

        _filesServiceMock
            .DeleteFiles(Arg.Any<IEnumerable<FileInfo>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IReadOnlyList<FilePath>, Error>(data));
    }

    public void SetupFailureFileServiceMock()
    {
        _filesServiceMock
            .UploadFiles(Arg.Any<IEnumerable<FileData>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IReadOnlyList<FilePath>, Error>(Errors.General.ValueNotFound()));
        
        _filesServiceMock
            .DeleteFiles(Arg.Any<IEnumerable<FileInfo>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IReadOnlyList<FilePath>, Error>(Errors.General.ValueNotFound()));
    }
}