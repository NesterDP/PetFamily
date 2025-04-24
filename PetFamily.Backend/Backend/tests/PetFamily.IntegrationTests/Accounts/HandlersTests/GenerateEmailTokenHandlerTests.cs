using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.GenerateEmailToken;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Accounts.HandlersTests;

public class GenerateEmailTokenHandlerTests : AccountsTestsBase
{
    private readonly ICommandHandler<string, GenerateEmailTokenCommand> _sut;

    public GenerateEmailTokenHandlerTests(AccountsTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<string, GenerateEmailTokenCommand>>();
    }

    [Fact]
    public async Task GenerateEmailToken_success_should_return_token()
    {
        // arrange
        string? EMAIL = "test@mail.com";
        string? USERNAME = "testUserName";
        string? PASSWORD = "Password121314s.";

        var user = await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);
        
        var command = new GenerateEmailTokenCommand(user.Id);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GenerateEmailToken_failure_should_return_failure_because_of_no_user_with_such_id_exist()
    {
        // arrange
        string? EMAIL = "test@mail.com";
        string? USERNAME = "testUserName";
        string? PASSWORD = "Password121314s.";

        await DataGenerator.SeedUserAsync(USERNAME, EMAIL, PASSWORD, UserManager, RoleManager);
        
        var command = new GenerateEmailTokenCommand(Guid.NewGuid());
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }
}