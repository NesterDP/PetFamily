using PetFamily.Core.Dto.Discussion;

namespace PetFamily.Discussions.Application.Abstractions;

public interface IReadDbContext
{
    IQueryable<DiscussionDto> Discussions { get; }
    IQueryable<MessageDto> Messages { get; }
}