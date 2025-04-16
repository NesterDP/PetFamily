using FluentAssertions;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.Events;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.UnitTests;

public class VolunteerRequestsTests
{
    [Fact]
    public void CreateVolunteerRequest_success_should_create_request_with_default_values()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;

        // Act
        var result = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);

        // Assert
        result.Id.Value.Should().NotBeEmpty();
        result.AdminId.Should().BeNull();
        result.UserId.Should().Be(userId);
        result.VolunteerInfo.Value.Should().Be(DEFAULT_TEXT);
        result.Status.Value.Should().Be(VolunteerRequestStatusEnum.Submitted);
        result.CreatedAt.Should().BeBefore(DateTime.UtcNow);
        result.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddHours(-1));
        result.RevisionComment.Should().BeNull();
    }

    // set Submitted //////////////////////////////////////////////////////////////////

    [Fact]
    public void SetSubmitted_failure_should_return_error_while_trying_to_set_Submitted_from_Submitted()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);

        // Act
        var result = request.SetSubmitted(volunteerInfo);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetSubmitted_failure_should_return_error_while_trying_to_set_Submitted_from_OnReview()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);

        // Act
        var result = request.SetSubmitted(volunteerInfo);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetSubmitted_Success_should_change_status_from_RevisionRequired_to_Submitted()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;
        var adminId = AdminId.NewAdminId();
        
        
        request.SetOnReview(adminId);
        request.SetRevisionRequired(adminId, revisionComment);

        // Act
        var result = request.SetSubmitted(volunteerInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        request.AdminId.Should().Be(adminId);
        request.RevisionComment.Should().Be(revisionComment);
        request.Status.Value.Should().Be(VolunteerRequestStatusEnum.Submitted);
    }

    [Fact]
    public void SetSubmitted_Failure_should_return_error_while_trying_to_set_Submitted_from_Rejected()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);
        request.SetRejected(adminId);

        // Act
        var result = request.SetSubmitted(volunteerInfo);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetSubmitted_failure_should_return_error_while_trying_to_set_Submitted_from_Approved()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);
        request.SetApproved(adminId);

        // Act
        var result = request.SetSubmitted(volunteerInfo);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    // set OnReview //////////////////////////////////////////////////////////////////

    [Fact]
    public void SetOnReview_success_should_change_status_from_Submitted_to_OnReview()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();

        // Act
        var result = request.SetOnReview(adminId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        request.AdminId.Should().Be(adminId);
        request.Status.Value.Should().Be(VolunteerRequestStatusEnum.OnReview);
    }

    [Fact]
    public void SetOnReview_failure_should_return_error_while_trying_to_set_OnReview_from_OnReview()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);

        // Act
        var result = request.SetOnReview(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetOnReview_failure_should_return_error_while_trying_to_set_OnReview_from_RevisionRequired()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;
        
        request.SetOnReview(adminId);
        request.SetRevisionRequired(adminId, revisionComment);

        // Act
        var result = request.SetOnReview(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetOnReview_failure_should_return_error_while_trying_to_set_OnReview_from_Rejected()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);
        request.SetRejected(adminId);

        // Act
        var result = request.SetOnReview(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetOnReview_failure_should_return_error_while_trying_to_set_OnReview_from_Approved()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);
        request.SetApproved(adminId);

        // Act
        var result = request.SetOnReview(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    // set RevisionRequired //////////////////////////////////////////////////////////////////
    
    [Fact]
    public void SetRevisionRequired_failure_should_return_error_while_trying_to_set_RevisionRequired_from_Submitted()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;

        // Act
        var result = request.SetRevisionRequired(adminId, revisionComment);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetRevisionRequired_success_should_change_status_from_OnReview_to_RevisionRequired()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;
        
        request.SetOnReview(adminId);
        
        // Act
        var result = request.SetRevisionRequired(adminId, revisionComment);

        // Assert
        result.IsSuccess.Should().BeTrue();
        request.AdminId.Should().Be(adminId);
        request.RevisionComment.Value.Should().Be(revisionComment.Value);
        request.Status.Value.Should().Be(VolunteerRequestStatusEnum.RevisionRequired);
    }

    [Fact]
    public void SetRevisionRequired_failure_should_return_error_while_trying_to_set_RevisionRequired_from_RevisionRequired()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;
        
        request.SetOnReview(adminId);
        request.SetRevisionRequired(adminId, revisionComment);

        // Act
        var result = request.SetRevisionRequired(adminId, revisionComment);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public void SetRevisionRequired_failure_should_return_error_while_trying_to_set_RevisionRequired_from_Rejected()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;

        request.SetOnReview(adminId);
        request.SetRejected(adminId);
        
        // Act
        var result = request.SetRevisionRequired(adminId, revisionComment);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetRevisionRequired_failure_should_return_error_while_trying_to_set_RevisionRequired_from_Approved()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;
        
        request.SetOnReview(adminId);
        request.SetApproved(adminId);

        // Act
        var result = request.SetRevisionRequired(adminId, revisionComment);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    // set Rejected

    [Fact]
    public void SetRejected_failure_should_return_error_while_trying_to_set_Rejected_from_Submitted()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);

        var adminId = AdminId.NewAdminId();

        // Act
        var result = request.SetRejected(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetRejected_success_should_change_status_from_OnReview_to_Rejected()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);

        // Act
        var result = request.SetRejected(adminId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        request.AdminId.Should().Be(adminId);
        request.Status.Value.Should().Be(VolunteerRequestStatusEnum.Rejected);
    }

    [Fact]
    public void SetRejected_failure_should_return_error_while_trying_to_set_Rejected_from_RevisionRequired()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;
        
        request.SetOnReview(adminId);
        request.SetRevisionRequired(adminId, revisionComment);

        // Act
        var result = request.SetRejected(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetRejected_failure_should_return_error_while_trying_to_set_Rejected_from_Rejected()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);
        request.SetRejected(adminId);

        // Act
        var result = request.SetRejected(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetRejected_failure_should_return_error_while_trying_to_set_Rejected_from_Approved()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);
        request.SetApproved(adminId);

        // Act
        var result = request.SetRejected(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    // set approved

    [Fact]
    public void SetApproved_failure_should_return_error_while_trying_to_set_Approved_from_Submitted()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();

        // Act
        var result = request.SetApproved(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetApproved_success_should_change_status_from_OnReview_to_Approved()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);

        // Act
        var result = request.SetApproved(adminId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        request.AdminId.Should().Be(adminId);
        request.Status.Value.Should().Be(VolunteerRequestStatusEnum.Approved);

        var domainEvent = request.DomainEvents.SingleOrDefault() as VolunteerRequestSentForApprovedEvent;
        request.DomainEvents.SingleOrDefault().Should().NotBeNull();
        domainEvent.Should().NotBeNull();
        domainEvent.UserId.Should().Be(userId);
    }

    [Fact]
    public void SetApproved_failure_should_return_error_while_trying_to_set_Approved_from_RevisionRequired()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REVISION_TEXT = "revision text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;
        
        request.SetOnReview(adminId);
        request.SetRevisionRequired(adminId, revisionComment);

        // Act
        var result = request.SetApproved(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetApproved_failure_should_return_error_while_trying_to_set_Approved_from_Rejected()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);
        request.SetRejected(adminId);

        // Act
        var result = request.SetApproved(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SetApproved_failure_should_return_error_while_trying_to_set_Approved_from_Approved()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(VolunteerRequestId.NewVolunteerRequestId(), userId, volunteerInfo);
        var adminId = AdminId.NewAdminId();
        
        request.SetOnReview(adminId);
        request.SetApproved(adminId);

        // Act
        var result = request.SetApproved(adminId);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}