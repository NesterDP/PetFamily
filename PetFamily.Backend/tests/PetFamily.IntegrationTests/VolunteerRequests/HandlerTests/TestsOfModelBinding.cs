using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;


public class TestsOfModelBinding : VolunteerRequestsTestsBase
{
    public TestsOfModelBinding(VolunteerRequestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Test_should_add_to_2_requests_to_database()
    {
        // arrange
        
        
        // act
        var result_1 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        var result_2 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);

        // assert
        WriteDbContext.VolunteerRequests.Count().Should().Be(2);
    }
    
    [Fact]
    public async Task Test_should_add_request_to_database_with_all_its_fields_being_non_null()
    {
        // Arrange
        var DEFAULT_TEXT = "default text";
        var REJECTION_TEXT = "rejection text";
        var userId = UserId.NewUserId();
        var volunteerInfo = VolunteerInfo.Create(DEFAULT_TEXT).Value;
        var request = new VolunteerRequest(userId, volunteerInfo);
        
        var adminId = AdminId.NewAdminId();
        var rejectionComment = RejectionComment.Create(REJECTION_TEXT).Value;
        request.SetRevisionRequired(adminId, rejectionComment);
        

        // act
        WriteDbContext.VolunteerRequests.Add(request);
        await WriteDbContext.SaveChangesAsync();
        
        // assert
        var result = await WriteDbContext.VolunteerRequests.FirstOrDefaultAsync();
        result.Id.Value.Should().Be(request.Id);
        result.AdminId.Should().Be(adminId);
        result.UserId.Should().Be(userId);
        result.VolunteerInfo.Value.Should().Be(DEFAULT_TEXT);
        result.Status.Value.Should().Be(VolunteerRequestStatusEnum.RevisionRequired);
        result.CreatedAt.Should().BeBefore(DateTime.UtcNow);
        result.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddHours(-1));
        result.RejectionComment.Value.Should().Be(rejectionComment.Value);
    }
}