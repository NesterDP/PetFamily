using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Discussions.Application.Commands.CreateDiscussion;
using PetFamily.IntegrationTests.Discussions.Heritage;
using PetFamily.IntegrationTests.General;
using PetFamily.VolunteerRequests.Application.Commands.CreateVolunteerRequest;

namespace PetFamily.IntegrationTests.Discussions.HandlerTests;

public class CreateDiscussionHandlerTests : DiscussionsTestsBase
{
    private readonly ICommandHandler<Guid, CreateDiscussionCommand> _sut;

    public CreateDiscussionHandlerTests(DiscussionsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateDiscussionCommand>>();
    }

    [Fact]
    public async Task CreateDiscussion_success_should_create_new_discussion()
    {
        // arrange
        var relationId = Guid.NewGuid();
        List<Guid> userIds = [Guid.NewGuid(), Guid.NewGuid()];
        
        var command = new CreateDiscussionCommand(relationId, userIds);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        WriteDbContext.Discussions.Count().Should().Be(1);
        var discussion = await WriteDbContext.Discussions.FirstOrDefaultAsync(d => d.Id == result.Value);
        discussion.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CreateDiscussion_failure_should_return_error_because_discussion_with_such_relationId_already_exists()
    {
        // arrange
        int USER_COUNT = 2;
        var existedDiscussion = await DataGenerator.SeedDiscussion(WriteDbContext, USER_COUNT);
        List<Guid> userIds = [Guid.NewGuid(), Guid.NewGuid()];
        
        var command = new CreateDiscussionCommand(existedDiscussion.RelationId, userIds);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();
        WriteDbContext.Discussions.Count().Should().Be(1);
        WriteDbContext.Discussions.FirstOrDefault(d => d.Id == existedDiscussion.Id).Should().NotBeNull();
    }
    
    [Fact]
    public async Task CreateDiscussion_failure_should_return_error_because_not_enough_users_in_discussion()
    {
        // arrange
        var relationId = Guid.NewGuid();
        List<Guid> userIds = [Guid.NewGuid()];
        
        var command = new CreateDiscussionCommand(relationId, userIds);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public async Task CreateDiscussion_failure_should_return_error_because_too_many_users_in_discussion()
    {
        // arrange
        var relationId = Guid.NewGuid();
        List<Guid> userIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
        
        var command = new CreateDiscussionCommand(relationId, userIds);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();
    }
}