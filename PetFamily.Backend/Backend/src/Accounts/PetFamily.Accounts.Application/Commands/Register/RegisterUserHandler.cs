using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Application.Commands.Register;

public class RegisterUserHandler : ICommandHandler<string, RegisterUserCommand>
{
    private const string SUCCESS_MESSAGE = "Successfully registered";
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAccountManager _accountManager;
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IAccountManager accountManager,
        IOutboxRepository outboxRepository,
        ILogger<RegisterUserHandler> logger,
        [FromKeyedServices(UnitOfWorkSelector.Accounts)]
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _accountManager = accountManager;
        _outboxRepository = outboxRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var participantRole = await _roleManager.FindByNameAsync(DomainConstants.PARTICIPANT)
                              ?? throw new ApplicationException("Could not find participant role");

        var participant = User.CreateParticipant(
            command.UserName,
            command.Email,
            FullName.Create("DefaultFirstName", "DefaultLastName").Value,
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

                await _accountManager.CreateParticipantAccount(participantAccount);

                await EnqueueEventAsync(participant.Value.Id, cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Successfully created participant with UserName = {UserName}", command.UserName);

                return SUCCESS_MESSAGE;
            }

            var errors = result.Errors
                .Select(e => Errors.General.Failure(e.Code, e.Description)).ToList();
            return new ErrorList(errors);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogInformation("Failed create participant with UserName = {0}", command.UserName);
            _logger.LogError(e, e.Message);
            return Errors.General.Failure("server.internal", "transaction failure").ToErrorList();
        }
    }

    private async Task EnqueueEventAsync(Guid userId, CancellationToken cancellationToken)
    {
        var integrationEvent = new Contracts.Messaging.UserWasRegisteredEvent(userId);

        await _outboxRepository.Add(integrationEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}