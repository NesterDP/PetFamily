using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Discussions.Application.Commands.RemoveMessage;
using PetFamily.IntegrationTests.Discussions.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Discussions.HandlerTests;

public class RemoveMessageHandlerTests : DiscussionsTestsBase
{
    private readonly ICommandHandler<Guid, RemoveMessageCommand> _sut;

    public RemoveMessageHandlerTests(DiscussionsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, RemoveMessageCommand>>();
    }

    [Fact]
    public async Task RemoveMessage_success_should_remove_message()
    {
        // arrange
        int USER_COUNT = 2;
        var discussion = await DataGenerator.SeedDiscussion(WriteDbContext, USER_COUNT);
        var message1 = DataGenerator.CreateMessage(discussion.UserIds[0]);
        var message2 = DataGenerator.CreateMessage(discussion.UserIds[0]);
        var message3 = DataGenerator.CreateMessage(discussion.UserIds[0]);
        discussion.Messages.Add(message1);
        discussion.Messages.Add(message2);
        discussion.Messages.Add(message3);
        await WriteDbContext.SaveChangesAsync();

        var command = new RemoveMessageCommand(discussion.RelationId, message2.Id, discussion.UserIds[0]);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        var discussionResult = await WriteDbContext.Discussions.FirstOrDefaultAsync(d => d.Id == discussion.Id);
        discussionResult.Should().NotBeNull();
        discussionResult.Messages.Count().Should().Be(2);
        discussionResult.Messages.Should().NotContain(m => m.Id == message2.Id);
    }
    
    [Fact]
    public async Task RemoveMessage_failure_should_return_error_because_deleter_is_not_creator_of_message()
    {
        // arrange
        int USER_COUNT = 2;
        var discussion = await DataGenerator.SeedDiscussion(WriteDbContext, USER_COUNT);
        var message1 = DataGenerator.CreateMessage(discussion.UserIds[0]);
        var message2 = DataGenerator.CreateMessage(discussion.UserIds[0]);
        var message3 = DataGenerator.CreateMessage(discussion.UserIds[0]);
        discussion.Messages.Add(message1);
        discussion.Messages.Add(message2);
        discussion.Messages.Add(message3);
        await WriteDbContext.SaveChangesAsync();
        
        var command = new RemoveMessageCommand(discussion.RelationId, message2.Id, discussion.UserIds[1]);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
        var discussionResult = await WriteDbContext.Discussions.FirstOrDefaultAsync(d => d.Id == discussion.Id);
        discussionResult.Should().NotBeNull();
        discussionResult.Messages.Count().Should().Be(3);
    }
}