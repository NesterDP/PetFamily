using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Commands.Register;

public class RegisterUserHandler : ICommandHandler<string, RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RegisterUserHandler> _logger;

    public RegisterUserHandler(
        UserManager<User> userManager,
        ILogger<RegisterUserHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = command.Email,
            UserName = command.UserName
        };

        var result = await _userManager.CreateAsync(user, command.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("Successfully created user with UserName = {0}", command.UserName);
            return "Successfully registered";
        }
        
        var errors = result.Errors
            .Select(e => Errors.General.Failure(e.Code, e.Description)).ToList();
        return new ErrorList(errors);

    }
    
}