using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Discussions.Application.Abstractions;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.Discussions.Infrastructure.DbContexts;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Discussions.Infrastructure.Repositories;

public class DiscussionsRepository : IDiscussionsRepository
{
    private readonly WriteDbContext _context;

    public DiscussionsRepository(WriteDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Guid> AddAsync(Discussion discussion, CancellationToken cancellationToken = default)
    {
        await _context.Discussions.AddAsync(discussion, cancellationToken);
        return discussion.Id.Value;
    }

    public Guid Save(Discussion discussion, CancellationToken cancellationToken = default)
    {
        _context.Discussions.Attach(discussion);
        return discussion.Id.Value;
    }
    
    public Guid Delete(Discussion discussion, CancellationToken cancellationToken = default)
    {
        _context.Discussions.Remove(discussion);
        return discussion.Id.Value;
    }

    public async Task<Result<Discussion, Error>> GetByIdAsync(DiscussionId id,
        CancellationToken cancellationToken = default)
    {
        var discussion = await _context.Discussions
            .Include(d => d.Messages)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (discussion == null)
            return Errors.General.ValueNotFound();

        return discussion;
    }
    
    public async Task<Result<Discussion, Error>> GetByRelationIdAsync(
        RelationId relationId,
        CancellationToken cancellationToken = default)
    {
        var discussion = await _context.Discussions
            .Include(d => d.Messages)
            .FirstOrDefaultAsync(v => v.RelationId == relationId, cancellationToken);
        
        if (discussion == null)
            return Errors.General.ValueNotFound();

        return discussion;
    }
}