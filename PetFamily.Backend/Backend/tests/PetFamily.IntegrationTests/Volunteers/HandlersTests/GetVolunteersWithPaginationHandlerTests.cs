using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Core.Models;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Volunteers.Application.Queries.GetVolunteersWithPagination;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class GetVolunteersWithPaginationHandlerTests : VolunteerTestsBase
{
    private readonly IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery> _sut;

    public GetVolunteersWithPaginationHandlerTests(VolunteerTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery>>();
    }

    [Fact]
    public async Task GetVolunteerWithPagination_should_return_all_volunteers()
    {
        // arrange
        const int VOLUNTEERS_COUNT = 16;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;
        var volunteers = await DataGenerator.SeedVolunteers(VolunteersWriteDbContext, VOLUNTEERS_COUNT);
        var volunteersIds = volunteers.Select(v => v.Id.Value).ToHashSet();
        var query = new GetVolunteersWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(VOLUNTEERS_COUNT);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items.Select(i => i.Id).ToHashSet().Should().BeEquivalentTo(volunteersIds);
    }

    [Fact]
    public async Task GetVolunteerWithPagination_should_return_only_1_non_soft_deleted_volunteer()
    {
        // arrange
        const int VOLUNTEERS_COUNT = 16;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;
        var volunteers = await DataGenerator.SeedVolunteers(VolunteersWriteDbContext, VOLUNTEERS_COUNT);

        // all previously seeded volunteers are being soft deleted
        foreach (var volunteer in volunteers)
        {
            volunteer.Delete();
        }

        // non soft deleted volunteer
        var nonSoftDeletedVolunteer = await DataGenerator.SeedVolunteer(VolunteersWriteDbContext);
        var query = new GetVolunteersWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(1);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items[0].Id.Should().Be(nonSoftDeletedVolunteer.Id);
    }

    [Fact]
    public async Task GetVolunteerWithPagination_should_return_empty_items_list_if_there_are_no_volunteers()
    {
        // arrange
        const int PAGE_SIZE = 16;
        const int PAGE = 1;
        var query = new GetVolunteersWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVolunteerWithPagination_should_return_has_no_previous_page_and_has_next_page()
    {
        // arrange
        const int VOLUNTEERS_COUNT = 16;
        const int PAGE_SIZE = 8;
        const int PAGE = 1;
        await DataGenerator.SeedVolunteers(VolunteersWriteDbContext, VOLUNTEERS_COUNT);
        var query = new GetVolunteersWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(VOLUNTEERS_COUNT);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetVolunteerWithPagination_should_have_both_previous_page_and_next_page()
    {
        // arrange
        const int VOLUNTEERS_COUNT = 16;
        const int PAGE_SIZE = 4;
        const int PAGE = 3;
        await DataGenerator.SeedVolunteers(VolunteersWriteDbContext, VOLUNTEERS_COUNT);
        var query = new GetVolunteersWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetVolunteerWithPagination_should_have_only_previous_page()
    {
        // arrange
        const int VOLUNTEERS_COUNT = 16;
        const int PAGE_SIZE = 4;
        const int PAGE = 4;
        await DataGenerator.SeedVolunteers(VolunteersWriteDbContext, VOLUNTEERS_COUNT);
        var query = new GetVolunteersWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }
}