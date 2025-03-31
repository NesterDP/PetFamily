using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Discussion;
using PetFamily.Discussions.Application.Queries.GetDiscussion;
using PetFamily.IntegrationTests.Discussions.Heritage;
using PetFamily.IntegrationTests.General;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.IntegrationTests.Discussions.HandlerTests;

public class GetDiscussionHandlerTests : DiscussionsTestsBase
{
    private readonly IQueryHandler<Result<DiscussionDto, ErrorList>, GetDiscussionQuery> _sut;

    public GetDiscussionHandlerTests(DiscussionsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<Result<DiscussionDto, ErrorList>, GetDiscussionQuery>>();
    }

    [Fact]
    public async Task GetDiscussion_success_should_return_discussion_with_all_its_messages()
    {
        // arrange
        var USER_COUNT = 2;
        var TEXT_1 = "First test message";
        var TEXT_2 = "Second test message";
        var TEXT_3 = "Third test message";
        var discussion = await DataGenerator.SeedDiscussion(WriteDbContext, USER_COUNT);
        var message1 = DataGenerator.CreateMessage(discussion.UserIds[0], TEXT_1);
        var message2 = DataGenerator.CreateMessage(discussion.UserIds[1], TEXT_2);
        var message3 = DataGenerator.CreateMessage(discussion.UserIds[0], TEXT_3);
        discussion.Messages.Add(message1);
        discussion.Messages.Add(message2);
        discussion.Messages.Add(message3);
        await WriteDbContext.SaveChangesAsync();

        var query = new GetDiscussionQuery(discussion.RelationId);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        discussion.Messages.Count.Should().Be(3);
        discussion.Messages[0].Text.Value.Should().Be(TEXT_1);
        discussion.Messages[0].UserId.Should().Be(discussion.UserIds[0]);
        discussion.Messages[1].Text.Value.Should().Be(TEXT_2);
        discussion.Messages[1].UserId.Should().Be(discussion.UserIds[1]);
        discussion.Messages[2].Text.Value.Should().Be(TEXT_3);
        discussion.Messages[2].UserId.Should().Be(discussion.UserIds[0]);
    }
}