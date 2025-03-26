using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;


public class TestsOfModelBinding : VolunteerRequestsTestsBase
{
    public TestsOfModelBinding(VolunteerRequestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SoftDeletePet_success_should_delete_2_pets_that_were_soft_deleted_with_expired_lifetime()
    {
        // arrange
        var DEFAULT_TEXT = "default text";

        // act
        var result_1 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        var result_2 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);

        // assert
        WriteDbContext.VolunteerRequests.Count().Should().Be(2);
        var request = await WriteDbContext.VolunteerRequests.FirstOrDefaultAsync();
        request.Should().NotBeNull();
        request.AdminId.Should().BeNull();
        request.VolunteerInfo.Value.Should().Be(DEFAULT_TEXT);
    }
}