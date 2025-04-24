using MassTransit;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Models;
using PetFamily.Species.Contracts.Messaging;
using PetFamily.Volunteers.Contracts;
using PetFamily.Volunteers.Contracts.Requests;

namespace PetFamily.Volunteers.Infrastructure.Consumers;

public class BreedToPetExistenceEventConsumer : IConsumer<BreedToPetExistenceEvent>
{
    private readonly IBreedToPetExistenceContract _contract;

    public BreedToPetExistenceEventConsumer(
       IBreedToPetExistenceContract contract)
    {
        _contract = contract;
    }

    public async Task Consume(ConsumeContext<BreedToPetExistenceEvent> context)
    {
        var request = new BreedToPetExistenceRequest(context.Message.BreedId);

        var result = await _contract.BreedToPetExistence(request);

        string? response = DomainConstants.OK;
        if (result.IsFailure)
            response = result.Error.Message;

        await context.RespondAsync(new ResponseWrapper(response));
    }
}