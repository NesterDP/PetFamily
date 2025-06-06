using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.Login;
using PetFamily.Accounts.Contracts.Responses;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;
using PetFamily.SharedKernel.Constants;

namespace PetFamily.IntegrationTests.Accounts.HandlersTests;

public class LoginUserHandlerTests : AccountsTestsBase
{
    private readonly ICommandHandler<LoginResponse, LoginUserCommand> _sut;

    public LoginUserHandlerTests(AccountsTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginResponse, LoginUserCommand>>();
    }

    [Fact]
    public async Task LoginUser_success_should_return_access_and_refresh_tokens()
    {
        // arrange
        const string EMAIL = "test@mail.com";
        const string USERNAME = "testUserName";
        const string PASSWORD = "Password121314s.";

        var user = await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);

        var command = new LoginUserCommand(EMAIL, PASSWORD);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBeEmpty();

        // returned JTI
        var userClaims = await TokenProvider.GetUserClaims(result.Value.AccessToken);
        string userJtiString = userClaims.Value.FirstOrDefault(c => c.Type == CustomClaims.JTI)!.Value;

        // refresh session was created and filled with correct data
        string key = CacheConstants.REFRESH_SESSIONS_PREFIX + result.Value.RefreshToken;
        var refreshSession = await CacheService.GetAsync<RefreshSession>(key, CancellationToken.None);

        refreshSession!.UserId.Should().Be(user.Id);
        refreshSession.Jti.Should().Be(userJtiString);
        refreshSession.RefreshToken.Should().Be(result.Value.RefreshToken);
    }

    [Fact]
    public async Task Login_failure_should_return_failure_because_of_no_user_exist_with_such_login_data()
    {
        // arrange
        const string EMAIL = "test@mail.com";
        const string USERNAME = "testUserName";
        const string PASSWORD = "Password121314s.";

        await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);

        var command = new LoginUserCommand(EMAIL, PASSWORD + PASSWORD);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }
}