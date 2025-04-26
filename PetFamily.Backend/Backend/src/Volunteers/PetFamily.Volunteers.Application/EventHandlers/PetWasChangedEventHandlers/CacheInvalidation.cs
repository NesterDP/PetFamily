using MediatR;
using PetFamily.Core.Caching;
using PetFamily.SharedKernel.Constants;
using PetFamily.Volunteers.Domain.Events;

namespace PetFamily.Volunteers.Application.EventHandlers.PetWasChangedEventHandlers;

public class CacheInvalidation : INotificationHandler<PetWasChangedEvent>
{
    private readonly ICacheService _cacheService;

    public CacheInvalidation(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(PetWasChangedEvent domainEvent, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheConstants.PETS_PREFIX, cancellationToken);
    }
}