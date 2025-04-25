using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Discussions.Application.Commands.CloseDiscussion;
using PetFamily.Discussions.Domain.ValueObjects;
using PetFamily.IntegrationTests.Discussions.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Discussions.HandlerTests;

public class CloseDiscussionHandlerTests : DiscussionsTestsBase
{
    private readonly ICommandHandler<Guid, CloseDiscussionCommand> _sut;

    public CloseDiscussionHandlerTests(DiscussionsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CloseDiscussionCommand>>();
    }

    [Fact]
    public async Task CloseDiscussion_success_should_close_discussion()
    {
        // arrange
        const int USER_COUNT = 2;
        var discussion = await DataGenerator.SeedDiscussion(WriteDbContext, USER_COUNT);

        var command = new CloseDiscussionCommand(discussion.RelationId, discussion.UserIds[0]);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        var discussionResult = await WriteDbContext.Discussions.FirstOrDefaultAsync(d => d.Id == result.Value);
        discussionResult.Should().NotBeNull();
        discussionResult!.Status.Value.Should().Be(DiscussionStatusEnum.Closed);
    }

    [Fact]
    public async Task CloseDiscussion_failure_should_return_error_because_user_is_not_a_member_of_discussion()
    {
        // arrange
        const int USER_COUNT = 2;
        var discussion = await DataGenerator.SeedDiscussion(WriteDbContext, USER_COUNT);

        var command = new CloseDiscussionCommand(discussion.RelationId, Guid.NewGuid());

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
        var discussionResult = await WriteDbContext.Discussions.FirstOrDefaultAsync(d => d.Id == discussion.Id);
        discussionResult.Should().NotBeNull();
        discussionResult!.Status.Value.Should().Be(DiscussionStatusEnum.Opened);
    }
}