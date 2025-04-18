using MassTransit;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Models;
using PetFamily.Species.Contracts.Messaging;
using PetFamily.Volunteers.Contracts;
using PetFamily.Volunteers.Contracts.Requests;

namespace PetFamily.Volunteers.Infrastructure.Consumers;

public class SpeciesToPetExistenceEventConsumer : IConsumer<SpeciesToPetExistenceEvent>
{
    private readonly ISpeciesToPetExistenceContract _contract;

    public SpeciesToPetExistenceEventConsumer(
        ISpeciesToPetExistenceContract contract)
    {
        _contract = contract;
    }

    public async Task Consume(ConsumeContext<SpeciesToPetExistenceEvent> context)
    {
        var request = new SpeciesToPetExistenceRequest(context.Message.SpeciesId);

        var result = await _contract.SpeciesToPetExistence(request);

        var response = DomainConstants.OK;
        if (result.IsFailure)
            response = result.Error.Message;

        await context.RespondAsync(new ResponseWrapper(response));
    }
}