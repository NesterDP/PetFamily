using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;
using PetFamily.VolunteerRequests.Application.Commands.CreateVolunteerRequest;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;

public class CreateVolunteerRequestTests : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerRequestCommand> _sut;

    public CreateVolunteerRequestTests(VolunteerRequestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerRequestCommand>>();
    }

    [Fact]
    public async Task CreateVolunteerRequest_success_should_create_new_volunteerRequest()
    {
        // arrange
        var request = DataGenerator.CreateVolunteerRequest();

        var command = new CreateVolunteerRequestCommand(request.UserId, request.VolunteerInfo.Value);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        WriteDbContext.VolunteerRequests.Count().Should().Be(1);
    }

    [Fact]
    public async Task CreateVolunteerRequest_failure_should_return_error_because_user_have_Submitted_request()
    {
        // arrange
        var userId = Guid.NewGuid();
        const string VOLUNTEER_INFO = "test info";

        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext, userId);

        var command = new CreateVolunteerRequestCommand(userId, VOLUNTEER_INFO);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
        WriteDbContext.VolunteerRequests.Count().Should().Be(1);
        WriteDbContext.VolunteerRequests.First().Id.Should().Be(seededRequest.Id);
    }

    [Fact]
    public async Task CreateVolunteerRequest_failure_should_return_error_because_user_have_OnReview_request()
    {
        // arrange
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        const string VOLUNTEER_INFO = "test info";

        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext, userId);
        seededRequest.SetOnReview(adminId);
        await WriteDbContext.SaveChangesAsync();

        var command = new CreateVolunteerRequestCommand(userId, VOLUNTEER_INFO);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
        WriteDbContext.VolunteerRequests.Count().Should().Be(1);
        WriteDbContext.VolunteerRequests.First().Id.Should().Be(seededRequest.Id);
    }

    [Fact]
    public async Task CreateVolunteerRequest_failure_should_return_error_because_user_have_RevisionRequired_request()
    {
        // arrange
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        const string VOLUNTEER_INFO = "test info";
        const string REVISION_TEXT = "revision text";

        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext, userId);
        var revisionComment = RevisionComment.Create(REVISION_TEXT).Value;
        seededRequest.SetOnReview(adminId);
        seededRequest.SetRevisionRequired(adminId, revisionComment);
        await WriteDbContext.SaveChangesAsync();

        var command = new CreateVolunteerRequestCommand(userId, VOLUNTEER_INFO);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
        WriteDbContext.VolunteerRequests.Count().Should().Be(1);
        WriteDbContext.VolunteerRequests.First().Id.Should().Be(seededRequest.Id);
    }

    [Fact]
    public async Task CreateVolunteerRequest_success_should_create_new_request_while_having_expired_rejected_request()
    {
        // arrange
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        const string VOLUNTEER_INFO = "test info";
        const int DAYS = -10;

        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext, userId);
        seededRequest.SetOnReview(adminId);
        seededRequest.SetRejected(adminId);
        var propertyInfo = typeof(VolunteerRequest).GetProperty("RejectedAt");
        propertyInfo?.SetValue(seededRequest, DateTime.UtcNow.AddDays(DAYS));
        await WriteDbContext.SaveChangesAsync();

        var command = new CreateVolunteerRequestCommand(userId, VOLUNTEER_INFO);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        WriteDbContext.VolunteerRequests.Count().Should().Be(2);
        var addedRequest = await WriteDbContext.VolunteerRequests
            .FirstOrDefaultAsync(r => r.Id == result.Value);

        addedRequest.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateVolunteerRequest_failure_should_return_error_because_of_non_expired_rejected_request()
    {
        // arrange
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        const string VOLUNTEER_INFO = "test info";

        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext, userId);
        seededRequest.SetOnReview(adminId);
        seededRequest.SetRejected(adminId);
        await WriteDbContext.SaveChangesAsync();

        var command = new CreateVolunteerRequestCommand(userId, VOLUNTEER_INFO);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
        WriteDbContext.VolunteerRequests.Count().Should().Be(1);
        WriteDbContext.VolunteerRequests.First().Id.Should().Be(seededRequest.Id);
    }

    // in case if user lost its volunteer role and trying to apply for being volunteer again
    [Fact]
    public async Task CreateVolunteerRequest_success_should_create_request_while_having_approved_request()
    {
        // arrange
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        const string VOLUNTEER_INFO = "test info";

        var seededRequest = await DataGenerator.SeedVolunteerRequest(WriteDbContext, userId);
        seededRequest.SetOnReview(adminId);
        seededRequest.SetApproved(adminId);
        await WriteDbContext.SaveChangesAsync();

        var command = new CreateVolunteerRequestCommand(userId, VOLUNTEER_INFO);

        // act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        WriteDbContext.VolunteerRequests.Count().Should().Be(2);
        var addedRequest = await WriteDbContext.VolunteerRequests
            .FirstOrDefaultAsync(r => r.Id == result.Value);

        addedRequest.Should().NotBeNull();
    }
}