using MediatR;
using PetFamily.SharedKernel.Abstractions;

namespace PetFamily.SharedKernel.Extensions;

public static class MediatorExtensions
{
    public static async Task PublishDomainEvents<TId>(
        this IPublisher publisher,
        DomainEntity<TId> entity,
        CancellationToken cancellationToken = default)
        where TId : IComparable<TId>
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }

        entity.ClearDomainEvents();
    }
}