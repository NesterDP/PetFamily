using AutoFixture;
using FileService.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.CompleteUploadAvatar;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.IntegrationTests.Accounts.HandlersTests;

public class CompleteUploadAvatarHandlerTests : AccountsTestsBase
{
    private readonly ICommandHandler<CompleteMultipartUploadResponse, CompleteUploadAvatarCommand> _sut;

    public CompleteUploadAvatarHandlerTests(AccountsTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<ICommandHandler<CompleteMultipartUploadResponse, CompleteUploadAvatarCommand>>();
    }

    [Fact]
    public async Task CompleteUploadAvatar_success_should_add_avatar_to_user_without_avatar()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Password121314s.";
        var newAvatarId = Guid.NewGuid();
        
        var user = await DataGenerator.SeedUserAsync(
            USERNAME,
            EMAIL,
            PASSWORD,
            UserManager,
            RoleManager,
            AccountManager);

        var command = new CompleteUploadAvatarCommand(user.Id, Fixture.Create<CompleteUploadFileDto>());
        
        Factory.SetupSuccessCompleteMultipartMock([newAvatarId]);

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MultipartCompleteInfos.Count.Should().Be(1);
        
        // user avatar is saved in database of main application
        var userResult = await UserManager.FindByEmailAsync(EMAIL);
        userResult.Should().NotBeNull();
        userResult.Avatar.Id.Should().Be(newAvatarId);
    }

    [Fact]
    public async Task CompleteUploadAvatar_success_should_replace_avatar_of_user_database()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Password121314s.";
        var newAvatarId = Guid.NewGuid();
        var oldAvatarId = Guid.NewGuid();
        
        var user = await DataGenerator.SeedUserAsync(
            USERNAME,
            EMAIL,
            PASSWORD,
            UserManager,
            RoleManager,
            AccountManager);
        
        user.Avatar = Avatar.Create(oldAvatarId, "image/jpg").Value;
        await AccountsDbContext.SaveChangesAsync();
        
        var command = new CompleteUploadAvatarCommand(user.Id, Fixture.Create<CompleteUploadFileDto>());
        
        Factory.SetupSuccessCompleteMultipartMock([newAvatarId]);
        Factory.SetupSuccessDeleteFilesByIdsMock([oldAvatarId]);

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MultipartCompleteInfos.Count.Should().Be(1);
        
        // user avatar is saved in database of main application
        var userResult = await UserManager.FindByEmailAsync(EMAIL);
        userResult.Should().NotBeNull();
        userResult.Avatar.Id.Should().Be(newAvatarId);
    }

    [Fact]
    public async Task CompleteUploadAvatar_failure_should_return_error_while_not_affecting_database()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Password121314s.";
        var oldAvatarId = Guid.NewGuid();
        
        var user = await DataGenerator.SeedUserAsync(
            USERNAME,
            EMAIL,
            PASSWORD,
            UserManager,
            RoleManager,
            AccountManager);
        
        user.Avatar = Avatar.Create(oldAvatarId, "image/jpg").Value;
        await AccountsDbContext.SaveChangesAsync();
        
        var command = new CompleteUploadAvatarCommand(user.Id, Fixture.Create<CompleteUploadFileDto>());
        
        Factory.SetupFailureCompleteMultipartMock();
        Factory.SetupFailureCompleteMultipartMock();

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsFailure.Should().BeTrue();
        
        // user avatar is not affected
        var userResult = await UserManager.FindByEmailAsync(EMAIL);
        userResult.Should().NotBeNull();
        userResult.Avatar.Id.Should().Be(oldAvatarId);
    }
}