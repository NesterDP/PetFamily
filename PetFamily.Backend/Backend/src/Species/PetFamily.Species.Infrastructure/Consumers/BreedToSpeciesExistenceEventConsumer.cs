using MassTransit;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Models;
using PetFamily.Species.Contracts;
using PetFamily.Species.Contracts.Requests;
using PetFamily.Volunteers.Contracts.Messaging;

namespace PetFamily.Species.Infrastructure.Consumers;

public class BreedToSpeciesExistenceEventConsumer : IConsumer<BreedToSpeciesExistenceEvent>
{
    private readonly IBreedToSpeciesExistenceContract _contract;

    public BreedToSpeciesExistenceEventConsumer(
        IBreedToSpeciesExistenceContract contract)
    {
        _contract = contract;
    }

    public async Task Consume(ConsumeContext<BreedToSpeciesExistenceEvent> context)
    {
        var request = new BreedToSpeciesExistenceRequest(context.Message.SpeciesId, context.Message.BreedId);

        var result = await _contract.BreedToSpeciesExistence(request);

        string? response = DomainConstants.OK;
        if (result.IsFailure)
            response = result.Error.Message;

        await context.RespondAsync(new ResponseWrapper(response));
    }
}