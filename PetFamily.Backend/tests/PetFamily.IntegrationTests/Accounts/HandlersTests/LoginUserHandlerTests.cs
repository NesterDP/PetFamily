using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.Login;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Accounts.HandlersTests;

public class LoginUserHandlerTests : AccountsTestsBase
{
    private readonly ICommandHandler<string, LoginUserCommand> _sut;

    public LoginUserHandlerTests(AccountsTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<string, LoginUserCommand>>();
    }

    [Fact]
    public async Task LoginUser_success_should_return_access_token()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Passowrd121314s.";
        var SUCCESS_MESSAGE = "Successfully registered";

        await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);
        
        var command = new LoginUserCommand(EMAIL, PASSWORD);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task Login_failure_should_return_failure_because_of_no_user_exist_with_such_login_data()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Passowrd121314s.";
        var SUCCESS_MESSAGE = "Successfully registered";

        await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);
        
        var command = new LoginUserCommand(EMAIL, PASSWORD + PASSWORD);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }
}