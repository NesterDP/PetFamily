using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.Register;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Accounts.HandlersTests;

public class RegisterUserHandlerTests : AccountsTestsBase
{
    private readonly ICommandHandler<string, RegisterUserCommand> _sut;

    public RegisterUserHandlerTests(AccountsTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<string, RegisterUserCommand>>();
    }

    [Fact]
    public async Task RegisterUser_success_should_return_success_message_string()
    {
        // arrange
        const string EMAIL = "test@mail.com";
        const string USERNAME = "testUserName";
        const string PASSWORD = "Password121314s.";
        const string SUCCESS_MESSAGE = "Successfully registered";
        var command = new RegisterUserCommand(EMAIL, USERNAME, PASSWORD);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(SUCCESS_MESSAGE);

        // Added user should be in database while not added shouldn't be
        var existingUser = await UserManager.FindByEmailAsync(EMAIL);
        var unexistingUser = await UserManager.FindByEmailAsync(EMAIL + EMAIL);
        existingUser.Should().NotBeNull();
        unexistingUser.Should().BeNull();
    }

    [Fact]
    public async Task RegisterUser_failure_should_return_failure_because_user_with_such_email_already_exists()
    {
        // arrange
        const string EMAIL = "test@mail.com";
        const string USERNAME = "testUserName";
        const string PASSWORD = "Password121314s.";
        await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);

        var command = new RegisterUserCommand(EMAIL, USERNAME, PASSWORD);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterUser_failure_should_return_failure_because_of_incorrect_email()
    {
        // arrange
        const string EMAIL = "test_sobaka_mail.com";
        const string USERNAME = "testUserName";
        const string PASSWORD = "Password121314s.";
        var command = new RegisterUserCommand(EMAIL, USERNAME, PASSWORD);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }
}