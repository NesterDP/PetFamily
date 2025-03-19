using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Commands.Register;

public class RegisterUserHandler : ICommandHandler<string, RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<RegisterUserHandler> _logger;
    private const string SUCCESS_MESSAGE = "Successfully registered";
    private const string DEFAULT_ROLE = "Participant";

    public RegisterUserHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ILogger<RegisterUserHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = command.Email,
            UserName = command.UserName,
            FullName = new FullNameDto("DefaultName", "DefaultLastName", "")
        };
        
        var result = await _userManager.CreateAsync(user, command.Password);
        if (result.Succeeded)
        {
            var result2 = await _userManager.AddToRoleAsync(user, DEFAULT_ROLE);
            _logger.LogInformation("Successfully created user with UserName = {0}", command.UserName);
            return SUCCESS_MESSAGE;
        }
        
        
        var errors = result.Errors
            .Select(e => Errors.General.Failure(e.Code, e.Description)).ToList();
        return new ErrorList(errors);

    }
}