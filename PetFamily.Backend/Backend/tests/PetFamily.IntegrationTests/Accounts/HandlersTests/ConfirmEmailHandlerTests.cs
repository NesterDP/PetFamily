using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.ConfirmEmail;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Accounts.HandlersTests;

public class ConfirmEmailHandlerTests : AccountsTestsBase
{
    private readonly ICommandHandler<string, ConfirmEmailCommand> _sut;

    public ConfirmEmailHandlerTests(AccountsTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<string, ConfirmEmailCommand>>();
    }

    [Fact]
    public async Task ConfirmEmail_success_should_return_token()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Password121314s.";

        var user = await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);

        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        
        var command = new ConfirmEmailCommand(user.Id, token);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task ConfirmEmail_failure_should_return_failure_because_token_is_invalid()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Password121314s.";

        var user = await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);
        
        const string token = "token";
        
        var command = new ConfirmEmailCommand(user.Id, token);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public async Task ConfirmEmail_failure_should_return_failure_because_of_no_user_with_such_id_exist()
    {
        // arrange
        var EMAIL = "test@mail.com";
        var USERNAME = "testUserName";
        var PASSWORD = "Password121314s.";

        var user = await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);
        
        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        
        var command = new ConfirmEmailCommand(Guid.NewGuid(), token);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }
}