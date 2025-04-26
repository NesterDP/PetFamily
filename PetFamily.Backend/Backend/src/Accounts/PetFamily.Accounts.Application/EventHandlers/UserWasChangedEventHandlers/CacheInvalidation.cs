using MediatR;
using PetFamily.Accounts.Domain.Events;
using PetFamily.Core.Caching;
using PetFamily.SharedKernel.Constants;

namespace PetFamily.Accounts.Application.EventHandlers.UserWasChangedEventHandlers;

public class CacheInvalidation : INotificationHandler<UserWasChangedEvent>
{
    private readonly ICacheService _cacheService;

    public CacheInvalidation(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(UserWasChangedEvent domainEvent, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheConstants.USERS_PREFIX, cancellationToken);
    }
}