using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.Discussions.Domain.ValueObjects;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Commands.RejectRequest;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;


public class RejectRequest : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<Guid, RejectRequestCommand> _sut;

    public RejectRequest(VolunteerRequestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, RejectRequestCommand>>();
    }
    
    [Fact]
    public async Task RejectRequest_success_should_set_request_status_to_Rejected_and_set_RejectedAt_property()
    {
        // arrange
        var adminId = Guid.NewGuid();
        
        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        seededRequest.SetOnReview(adminId);
        await WriteDbContext.SaveChangesAsync();
        
        List<UserId> UserIds = [adminId, seededRequest.UserId];
        var discussion = Discussion.Create(seededRequest.Id.Value, UserIds).Value;
        DiscussionsDbContext.Add(discussion);
        await DiscussionsDbContext.SaveChangesAsync();
        
        var command = new RejectRequestCommand(seededRequest.Id, adminId);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        var changedRequest = await WriteDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == seededRequest.Id);
        changedRequest.Should().NotBeNull();
        changedRequest.Status.Value.Should().Be(VolunteerRequestStatusEnum.Rejected);
        
        // rejecting should set RejectedAt property with value
        changedRequest.RejectedAt.Should().NotBeNull();
        
        // should close discussion that is related to request
        var updatedDiscussion = await DiscussionsDbContext.Discussions
            .FirstOrDefaultAsync(d => d.Id == discussion.Id);
        
        updatedDiscussion.Should().NotBeNull();
        updatedDiscussion.Status.Value.Should().Be(DiscussionStatusEnum.Closed);
    }
}