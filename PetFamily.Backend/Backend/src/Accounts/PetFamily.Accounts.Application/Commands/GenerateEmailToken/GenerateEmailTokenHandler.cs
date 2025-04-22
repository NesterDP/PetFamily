using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Commands.GenerateEmailToken;

public class GenerateEmailTokenHandler : ICommandHandler<string, GenerateEmailTokenCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<GenerateEmailTokenHandler> _logger;
    private readonly IValidator<GenerateEmailTokenCommand> _validator;

    public GenerateEmailTokenHandler(
        UserManager<User> userManager,
        ILogger<GenerateEmailTokenHandler> logger,
        IValidator<GenerateEmailTokenCommand> validator)
    {
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        GenerateEmailTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Errors.General.ValueNotFound("No user with such userId", true).ToErrorList();

        var result = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        _logger.LogInformation("Generated email confirmation token for user with ID = {ID}", user.Id);

        return result;
    }
}