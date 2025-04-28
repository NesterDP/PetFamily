using MassTransit;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Contracts;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.VolunteerRequests.Contracts.Messaging;
using PetFamily.Volunteers.Application.Commands.Create;

namespace PetFamily.Volunteers.Infrastructure.Consumers;

public class ApprovedRequestVolunteersConsumer : IConsumer<VolunteerRequestWasApprovedEvent>
{
    private readonly ILogger<ApprovedRequestVolunteersConsumer> _logger;
    private readonly CreateVolunteerHandler _handler;
    private readonly IGetUserInfoByUserIdContract _contract;

    public ApprovedRequestVolunteersConsumer(
        ILogger<ApprovedRequestVolunteersConsumer> logger,
        CreateVolunteerHandler handler,
        IGetUserInfoByUserIdContract contract)
    {
        _logger = logger;
        _handler = handler;
        _contract = contract;
    }

    public async Task Consume(ConsumeContext<VolunteerRequestWasApprovedEvent> context)
    {
        var accountsRequest = new GetUserInfoByUserIdRequest(context.Message.UserId);

        var accountsResult = await _contract.GetUserInfo(accountsRequest);

        if (accountsResult.IsFailure)
            throw new Exception(accountsResult.Error);

        var fullNameDto = new FullNameDto(
            accountsResult.Value.FullName.FirstName,
            accountsResult.Value.FullName.LastName,
            accountsResult.Value.FullName.Surname);

        var createVolunteerDto = new CreateVolunteerDto(
            context.Message.UserId,
            fullNameDto,
            accountsResult.Value.Email,
            accountsResult.Value.PhoneNumber,
            accountsResult.Value.VolunteerAccount!.Experience);

        var command = new CreateVolunteerCommand(createVolunteerDto);
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        if (result.IsFailure)
            throw new Exception(result.Error.FirstOrDefault()!.Message);

        _logger.LogInformation("Successfully created volunteer with = {ID}", result.Value);
    }
}