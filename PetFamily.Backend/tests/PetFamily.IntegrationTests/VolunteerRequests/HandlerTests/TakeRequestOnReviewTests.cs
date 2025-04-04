using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;
using PetFamily.VolunteerRequests.Application.Commands.TakeRequestOnReview;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;


public class TakeRequestOnReviewTests : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<Guid, TakeRequestOnReviewCommand> _sut;

    public TakeRequestOnReviewTests(VolunteerRequestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, TakeRequestOnReviewCommand>>();
    }
    
    [Fact]
    public async Task TakeRequestOnReview_success_should_set_request_status_to_OnReview_and_create_discussion()
    {
        // arrange
        var adminId = Guid.NewGuid();
        
        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        
        var command = new TakeRequestOnReviewCommand(seededRequest.Id, adminId);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        var changedRequest = await WriteDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == seededRequest.Id);
        changedRequest.Should().NotBeNull();
        changedRequest.Status.Value.Should().Be(VolunteerRequestStatusEnum.OnReview);
        
        // setting request "OnReview" should create discussion
        var discussion = await DiscussionsDbContext.Discussions
            .FirstOrDefaultAsync(d => d.RelationId == result.Value);
        discussion.Should().NotBeNull();
    }
}