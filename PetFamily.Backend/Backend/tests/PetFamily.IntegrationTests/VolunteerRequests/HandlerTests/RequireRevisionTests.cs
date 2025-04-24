using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;
using PetFamily.VolunteerRequests.Application.Commands.RequireRevision;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;

public class RequireRevisionTests : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<Guid, RequireRevisionCommand> _sut;

    public RequireRevisionTests(VolunteerRequestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, RequireRevisionCommand>>();
    }
    
    [Fact]
    public async Task RequireRevision_success_should_set_request_status_to_RequireRevision_and_create_comment()
    {
        // arrange
        string? REVISION_TEXT = "REVISION TEXT";
        var adminId = Guid.NewGuid();
        
        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        seededRequest.SetOnReview(adminId);
        
        var command = new RequireRevisionCommand(seededRequest.Id, adminId, REVISION_TEXT);
        
        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        var changedRequest = await WriteDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == seededRequest.Id);
        changedRequest.Should().NotBeNull();
        changedRequest.Status.Value.Should().Be(VolunteerRequestStatusEnum.RevisionRequired);
        
        // setting request "OnReview" should create comment
        changedRequest.RevisionComment.Value.Should().Be(REVISION_TEXT);
    }
}