using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Application.Commands.Register;

public class RegisterUserHandler : ICommandHandler<string, RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IParticipantAccountManager _participantAccountManager;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    private const string SUCCESS_MESSAGE = "Successfully registered";

    public RegisterUserHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IParticipantAccountManager participantAccountManager,
        ILogger<RegisterUserHandler> logger,
        [FromKeyedServices(UnitOfWorkSelector.Accounts)] IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _participantAccountManager = participantAccountManager;
        _logger = logger;
        _unitOfWork = unitOfWork;

    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        
        var participantRole = await _roleManager.FindByNameAsync(ParticipantAccount.PARTICIPANT)
                        ?? throw new ApplicationException("Could not find participant role");

        var participant = User.CreateParticipant(
            command.UserName,
            command.Email,
            FullName.Create("DefaultFirstName", "DefaultLastName", null).Value,
            participantRole);

        if (participant.IsFailure)
            return participant.Error.ToErrorList();
        
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await _userManager.CreateAsync(participant.Value, command.Password);
            if (result.Succeeded)
            {
                var participantAccount = new ParticipantAccount(participant.Value);
        
                await _participantAccountManager.CreateParticipantAccount(participantAccount);
                
                transaction.Commit();
                _logger.LogInformation("Successfully created participant with UserName = {0}", command.UserName);
                return SUCCESS_MESSAGE;
            }
            var errors = result.Errors
                .Select(e => Errors.General.Failure(e.Code, e.Description)).ToList();
            return new ErrorList(errors);

        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.LogInformation("Failed create participant with UserName = {0}", command.UserName);
            _logger.LogError(e, e.Message);
            return Errors.General.Failure("server.internal", "transaction failure").ToErrorList();
        }
    }
}