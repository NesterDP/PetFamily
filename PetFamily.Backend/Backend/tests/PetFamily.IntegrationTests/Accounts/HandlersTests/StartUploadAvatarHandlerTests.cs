using FileService.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.StartUploadAvatar;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Accounts.HandlersTests;

public class StartUploadAvatarHandlerTests : AccountsTestsBase
{
    private readonly ICommandHandler<StartMultipartUploadResponse, StartUploadAvatarCommand> _sut;

    public StartUploadAvatarHandlerTests(AccountsTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<ICommandHandler<StartMultipartUploadResponse, StartUploadAvatarCommand>>();
    }

    [Fact]
    public async Task StartUploadAvatar_success_should_generate_starting_multipart_upload_data()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Password121314s.";

        var user = await DataGenerator.SeedUserAsync(
            USERNAME,
            EMAIL,
            PASSWORD,
            UserManager,
            RoleManager,
            AccountManager);

        var startUploadFileDto = new StartUploadFileDto("fileName", "image/jpg", 123);

        var command = new StartUploadAvatarCommand(
            user.Id,
            startUploadFileDto);

        Factory.SetupSuccessStartMultipartMock();

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MultipartStartInfos.Should().HaveCount(1);
    }

    [Fact]
    public async Task StartUploadAvatar_failure_should_return_error()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Password121314s.";

        var user = await DataGenerator.SeedUserAsync(
            USERNAME,
            EMAIL,
            PASSWORD,
            UserManager,
            RoleManager,
            AccountManager);

        var startUploadFileDto = new StartUploadFileDto("fileName", "image/jpg", 123);

        var command = new StartUploadAvatarCommand(
            user.Id,
            startUploadFileDto);

        Factory.SetupFailureStartMultipartMock();

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsFailure.Should().BeTrue();
    }
}