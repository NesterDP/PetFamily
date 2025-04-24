using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Discussions.Application.Commands.EditMessage;
using PetFamily.IntegrationTests.Discussions.Heritage;
using PetFamily.IntegrationTests.General;

namespace PetFamily.IntegrationTests.Discussions.HandlerTests;

public class EditMessageHandlerTests : DiscussionsTestsBase
{
    private readonly ICommandHandler<Guid, EditMessageCommand> _sut;

    public EditMessageHandlerTests(DiscussionsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, EditMessageCommand>>();
    }

    [Fact]
    public async Task EditMessage_success_should_edit_message()
    {
        // arrange
        int USER_COUNT = 2;
        string? EDITED_TEXT = "Edited text";
        string? INITIAL_TEXT = "Initial text";
        var discussion = await DataGenerator.SeedDiscussion(WriteDbContext, USER_COUNT);
        var message = DataGenerator.CreateMessage(discussion.UserIds[0], INITIAL_TEXT);
        discussion.Messages.Add(message);
        await WriteDbContext.SaveChangesAsync();

        var command = new EditMessageCommand(
            discussion.RelationId,
            message.Id,
            discussion.UserIds[0],
            EDITED_TEXT);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        var discussionResult = await WriteDbContext.Discussions.FirstOrDefaultAsync(d => d.Id == discussion.Id);
        discussionResult.Should().NotBeNull();
        discussionResult.Messages.Count().Should().Be(1);
        discussionResult.Messages.First().Text.Value.Should().Be(EDITED_TEXT);
    }

    [Fact] public async Task EditMessage_failure_should_return_error_because_editor_is_not_creator_of_message()
    {
        // arrange
        int USER_COUNT = 2;
        string? EDITED_TEXT = "Edited text";
        string? INITIAL_TEXT = "Initial text";
        var discussion = await DataGenerator.SeedDiscussion(WriteDbContext, USER_COUNT);
        var message = DataGenerator.CreateMessage(discussion.UserIds[0], INITIAL_TEXT);
        discussion.Messages.Add(message);
        await WriteDbContext.SaveChangesAsync();

        var command = new EditMessageCommand(
            discussion.RelationId,
            message.Id,
            discussion.UserIds[1], // member of discussion but doesn't own message
            EDITED_TEXT);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
        var discussionResult = await WriteDbContext.Discussions.FirstOrDefaultAsync(d => d.Id == discussion.Id);
        discussionResult.Should().NotBeNull();
        discussionResult.Messages.Count().Should().Be(1);
        discussionResult.Messages.First().Text.Value.Should().Be(INITIAL_TEXT);
    }
}