using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Commands.ConfirmEmail;

public class ConfirmEmailHandler : ICommandHandler<string, ConfirmEmailCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ConfirmEmailHandler> _logger;
    private readonly IValidator<ConfirmEmailCommand> _validator;

    public ConfirmEmailHandler(
        UserManager<User> userManager,
        ILogger<ConfirmEmailHandler> logger,
        IValidator<ConfirmEmailCommand> validator)
    {
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Errors.General.ValueNotFound("No user with such userId", true).ToErrorList();
        
        var result = await _userManager.ConfirmEmailAsync(user, command.Token);

        if (result.Succeeded == false)
            return Errors.General.Conflict("wasn't able to confirm email for user").ToErrorList();

        _logger.LogInformation("Confirmed email for user with ID = {ID}", user.Id);

        const string response = "Email is confirmed";
        
        return response;
    }
}