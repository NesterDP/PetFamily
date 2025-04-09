using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.VolunteerRequest;
using PetFamily.Core.Models;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;
using PetFamily.VolunteerRequests.Application.Queries.GetUnhandledRequests;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;


public class GetUnhandledRequestsTests : VolunteerRequestsTestsBase
{
    private readonly IQueryHandler<PagedList<VolunteerRequestDto>, GetUnhandledRequestsQuery> _sut;

    public GetUnhandledRequestsTests(VolunteerRequestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<VolunteerRequestDto>, GetUnhandledRequestsQuery>>();
    }
    
    [Fact]
    public async Task GetUnhandledRequests_should_return_3_unhandled_requests()
    {
        // arrange
        var PAGE = 1;
        var PAGE_SIZE = 3;
        var seededRequest1 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        var seededRequest2 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        var seededRequest3 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        var seededRequest4 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        
        var query = new GetUnhandledRequestsQuery(PAGE, PAGE_SIZE);
        
        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.TotalCount.Should().Be(4);
        result.Items.Count().Should().Be(3);
        result.Items.Should().Contain(v => v.Id == seededRequest1.Id);
        result.Items.Should().Contain(v => v.Id == seededRequest2.Id);
        result.Items.Should().Contain(v => v.Id == seededRequest3.Id);
        
        // correctness of binding to DTO
        result.Items.Should().Contain(v => v.UserId == seededRequest1.UserId);
        result.Items.Should().Contain(v => v.Status == seededRequest1.Status.Value.ToString());
        result.Items.Should().Contain(v => v.CreatedAt.Date == seededRequest1.CreatedAt.Date);
        result.Items.Should().Contain(v => v.VolunteerInfo == seededRequest1.VolunteerInfo.Value);
    }
}