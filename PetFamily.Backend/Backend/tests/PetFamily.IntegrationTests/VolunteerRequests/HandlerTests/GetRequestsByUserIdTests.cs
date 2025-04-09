using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.VolunteerRequest;
using PetFamily.Core.Models;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.VolunteerRequests.Heritage;
using PetFamily.VolunteerRequests.Application.Queries.GetRequestsByUserId;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.IntegrationTests.VolunteerRequests.HandlerTests;

public class GetRequestsByUserIdTests : VolunteerRequestsTestsBase
{
    private readonly IQueryHandler<PagedList<VolunteerRequestDto>, GetRequestsByUserIdQuery> _sut;

    public GetRequestsByUserIdTests(VolunteerRequestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<VolunteerRequestDto>, GetRequestsByUserIdQuery>>();
    }
    
    [Fact]
    public async Task GetRequestsByUserId_should_return_3_requests()
    {
        // arrange
        var PAGE = 1;
        var PAGE_SIZE = 3;
        
        var adminId = Guid.NewGuid();
        var seededRequest1 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        var seededRequest2 = await DataGenerator.SeedVolunteerRequest(WriteDbContext, seededRequest1.UserId);
        var seededRequest3 = await DataGenerator.SeedVolunteerRequest(WriteDbContext, seededRequest1.UserId);
        var seededRequest4 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        
        seededRequest1.SetOnReview(adminId);

        seededRequest2.SetOnReview(adminId);
        seededRequest2.SetApproved(adminId);

        seededRequest3.SetOnReview(adminId);
        seededRequest3.SetRejected(adminId);
        await WriteDbContext.SaveChangesAsync();
        
        var query = new GetRequestsByUserIdQuery(seededRequest1.UserId, PAGE, PAGE_SIZE);
        
        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.TotalCount.Should().Be(3);
        result.Items.Count().Should().Be(3);
        result.Items.Should().Contain(v => v.Id == seededRequest1.Id);
        result.Items.Should().Contain(v => v.Id == seededRequest2.Id);
        result.Items.Should().Contain(v => v.Id == seededRequest3.Id);
    }
    
    [Fact]
    public async Task GetRequestsByUserId_should_return_2_requests_because_of_filtration()
    {
        // arrange
        var PAGE = 1;
        var PAGE_SIZE = 3;
        
        var adminId = Guid.NewGuid();
        var seededRequest1 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        var seededRequest2 = await DataGenerator.SeedVolunteerRequest(WriteDbContext, seededRequest1.UserId);
        var seededRequest3 = await DataGenerator.SeedVolunteerRequest(WriteDbContext, seededRequest1.UserId);
        var seededRequest4 = await DataGenerator.SeedVolunteerRequest(WriteDbContext);
        
        seededRequest1.SetOnReview(adminId);

        seededRequest2.SetOnReview(adminId);
        seededRequest2.SetRejected(adminId);

        seededRequest3.SetOnReview(adminId);
        seededRequest3.SetRejected(adminId);
        
        await WriteDbContext.SaveChangesAsync();
        
        var query = new GetRequestsByUserIdQuery(
            seededRequest1.UserId,
            PAGE,
            PAGE_SIZE,
            VolunteerRequestStatusEnum.Rejected.ToString());
        
        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.TotalCount.Should().Be(2);
        result.Items.Count().Should().Be(2);
        result.Items.Should().NotContain(v => v.Id == seededRequest1.Id);
        result.Items.Should().Contain(v => v.Id == seededRequest2.Id);
        result.Items.Should().Contain(v => v.Id == seededRequest3.Id);
    }
}