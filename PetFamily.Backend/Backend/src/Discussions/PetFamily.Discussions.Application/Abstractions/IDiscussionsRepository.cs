using CSharpFunctionalExtensions;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Discussions.Application.Abstractions;

public interface IDiscussionsRepository
{
    Task<Guid> AddAsync(Discussion discussion, CancellationToken cancellationToken = default);

    Guid Save(Discussion discussion, CancellationToken cancellationToken = default);

    Guid Delete(Discussion discussion, CancellationToken cancellationToken = default);

    Task<Result<Discussion, Error>> GetByIdAsync(
        DiscussionId id,
        CancellationToken cancellationToken = default);

    Task<Result<Discussion, Error>> GetByRelationIdAsync(
        RelationId relationId,
        CancellationToken cancellationToken = default);
}