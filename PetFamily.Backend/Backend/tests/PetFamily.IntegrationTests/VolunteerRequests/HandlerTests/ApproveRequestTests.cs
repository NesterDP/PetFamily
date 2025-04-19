using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.Consumers;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Core.Abstractions;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.Discussions.Domain.ValueObjects;
using PetFamily.Discussions.Infrastructure.Consumers;
using PetFamily.Discussions.Infrastructure.DbContexts;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Commands.ApproveRequest;
using PetFamily.VolunteerRequests.Contracts.Messaging;
using PetFamily.VolunteerRequests.Domain.ValueObjects;
using ApprovedRequestConsumer = PetFamily.Discussions.Infrastructure.Consumers.ApprovedRequestConsumer;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;


public class ApproveRequestTests : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<Guid, ApproveRequestCommand> _sut;

    public ApproveRequestTests(VolunteerRequestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, ApproveRequestCommand>>();
    }
    
    [Fact]
    public async Task ApproveRequest_success_should_set_request_status_to_Approved_and_make_user_a_volunteer()
    {
        // arrange
        var adminId = Guid.NewGuid();
        var harness = Factory.Services.GetTestHarness();
        
        var fullname = FullName.Create("FirstName", "LastName", "Surname").Value;
        var USER_NAME = "TestUser";
        var EMAIL = "test@test.com";
        var role = await RoleManager.FindByNameAsync(DomainConstants.PARTICIPANT);
        var user = User.CreateParticipant(USER_NAME, EMAIL, fullname, role).Value;
        var creationResult = await UserManager.CreateAsync(user);
        
        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext, user.Id);
        seededRequest.SetOnReview(adminId);
        await WriteDbContext.SaveChangesAsync();
        
        List<UserId> UserIds = [adminId, seededRequest.UserId];
        var discussion = Discussion.Create(seededRequest.Id.Value, UserIds).Value;
        DiscussionsDbContext.Add(discussion);
        await DiscussionsDbContext.SaveChangesAsync();
        
        var command = new ApproveRequestCommand(seededRequest.Id, adminId);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        var changedRequest = await WriteDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == seededRequest.Id);
        changedRequest.Should().NotBeNull();
        changedRequest.Status.Value.Should().Be(VolunteerRequestStatusEnum.Approved);
        
        // receiving correct data BD
        await Task.Delay(InfrastructureConstants.OUTBOX_TASK_WORKING_INTERVAL_IN_SECONDS*2000);
        var consumer1 = harness.GetConsumerHarness<ApprovedRequestConsumer>();
        var consumer2 = harness.GetConsumerHarness<PetFamily.Accounts.Infrastructure.Consumers.ApprovedRequestConsumer>();

        await Task.WhenAll(
            consumer1.Consumed.Any<VolunteerRequestWasApprovedEvent>(),
            consumer2.Consumed.Any<VolunteerRequestWasApprovedEvent>()
        );
        
        await using var freshDiscussionsDbContext = new WriteDbContext(Factory._dbContainer.GetConnectionString());
        await using var freshAccountsDbContext = new AccountsDbContext(Factory._dbContainer.GetConnectionString());

       
        // should close discussion that is related to request
        var updatedDiscussion = await freshDiscussionsDbContext.Discussions
            .FirstOrDefaultAsync(d => d.Id == discussion.Id);
        
        updatedDiscussion.Should().NotBeNull();
        updatedDiscussion.Status.Value.Should().Be(DiscussionStatusEnum.Closed);
        
        // should create a volunteer account for user
        freshAccountsDbContext.VolunteerAccounts
            .FirstOrDefaultAsync(a => a.UserId == user.Id).Should().NotBeNull();
        
        // should make user a volunteer while not losing its participant role
        var userRoles = await UserManager.GetRolesAsync(user);
        userRoles.Count.Should().Be(2);
        userRoles.Should().Contain(DomainConstants.VOLUNTEER);
        userRoles.Should().Contain(DomainConstants.PARTICIPANT);
    }
}