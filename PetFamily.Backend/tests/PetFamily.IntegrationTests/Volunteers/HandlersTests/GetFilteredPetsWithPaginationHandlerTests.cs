using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;
using PetFamily.Application.Volunteers.Queries.GetVolunteersWithPagination;
using PetFamily.Domain.VolunteerManagment.ValueObjects.PetVO;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.SharedVO;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class GetFilteredPetsWithPaginationHandlerTests : VolunteerTestsBase
{
    private readonly GetFilteredPetsWithPaginationHandler _sut;

    public GetFilteredPetsWithPaginationHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<GetFilteredPetsWithPaginationHandler>();
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_all_pets()
    {
        // arrange
        var PETS_COUNT = 16;
        var PAGE_SIZE = 16;
        var PAGE = 1;
        var volunteer1 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT / 2);
        var volunteer2 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT / 2);
        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(PETS_COUNT);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_only_filtered_pets()
    {
        // arrange
        var PETS_COUNT = 16;
        var PAGE_SIZE = 16;
        var PAGE = 1;
        var volunteer1 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT / 2);
        var volunteer2 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT / 2);
        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE, volunteer1.Id);
        var volunteer1PetsHash = volunteer1.AllOwnedPets.Select(p => p.Id.Value).ToHashSet();

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(8);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items.Select(i => i.Id).ToHashSet().Should().BeEquivalentTo(volunteer1PetsHash);
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_pets_with_photos_being_ordered_by_main_goes_first()
    {
        // arrange
        List<Photo> photos1 =
        [
            new Photo(FilePath.Create("new_photo_1.jpg").Value),
            new Photo(FilePath.Create("new_photo_2.jpg").Value, true)
        ];
        List<Photo> photos2 =
        [
            new Photo(FilePath.Create("new_photo_1.jpg").Value, true),
            new Photo(FilePath.Create("new_photo_2.jpg").Value)
        ];
        var PETS_COUNT = 16;
        var PAGE_SIZE = 16;
        var PAGE = 1;
        var volunteer1 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT / 2);
        var volunteer2 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT / 2);
        volunteer1.AllOwnedPets[2].UpdatePhotos(photos1);
        volunteer2.AllOwnedPets[4].UpdatePhotos(photos2);

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(PETS_COUNT);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();

        foreach (var pet in result.Items)
            if (pet.Photos.Length != 0)
                pet.Photos[0].Main.Should().BeTrue();
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_pets_sorted_by_name_ascending_order()
    {
        // arrange
        var PETS_COUNT = 3;
        var PAGE_SIZE = 16;
        var PAGE = 1;
        var volunteer1 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT);
        volunteer1.AllOwnedPets[0].UpdateName(Name.Create("barks").Value);
        volunteer1.AllOwnedPets[1].UpdateName(Name.Create("clap").Value);
        volunteer1.AllOwnedPets[2].UpdateName(Name.Create("angel").Value);
        await WriteDbContext.SaveChangesAsync();

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE, SortBy: "name", SortDirection: "asc");

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(PETS_COUNT);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items[0].Name.Should().Be("angel");
        result.Items[1].Name.Should().Be("barks");
        result.Items[2].Name.Should().Be("clap");
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_pets_sorted_by_name_descending_order()
    {
        // arrange
        var PETS_COUNT = 3;
        var PAGE_SIZE = 16;
        var PAGE = 1;
        var volunteer1 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT);
        volunteer1.AllOwnedPets[0].UpdateName(Name.Create("barks").Value);
        volunteer1.AllOwnedPets[1].UpdateName(Name.Create("clap").Value);
        volunteer1.AllOwnedPets[2].UpdateName(Name.Create("angel").Value);
        await WriteDbContext.SaveChangesAsync();

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE, SortBy: "name", SortDirection: "desc");

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(PETS_COUNT);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items[0].Name.Should().Be("clap");
        result.Items[1].Name.Should().Be("barks");
        result.Items[2].Name.Should().Be("angel");
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_only_1_pet_the_rest_are_soft_deleted()
    {
        // arrange
        var PETS_COUNT = 3;
        var PAGE_SIZE = 16;
        var PAGE = 1;
        var volunteer1 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT);
        volunteer1.AllOwnedPets[0].Delete();
        volunteer1.AllOwnedPets[1].Delete();
        volunteer1.AllOwnedPets[2].UpdateName(Name.Create("angel").Value);
        await WriteDbContext.SaveChangesAsync();

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(1);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items[0].Name.Should().Be("angel");
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_only_1_pet_that_passed_all_possible_filters()
    {
        // arrange
        var PETS_COUNT = 16;
        var PAGE_SIZE = 16;
        var PAGE = 1;
        var MAX_AGE = 5;
        var MAX_WEIGHT = 30;
        var MAX_HEIGHT = 30;
        var volunteer1 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT);
        var volunteer2 = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PETS_COUNT);

        var pet = volunteer1.AllOwnedPets[0];
        pet.UpdateName(Name.Create("chip").Value);
        pet.UpdateDateOfBirth(DateOfBirth.Create(DateTime.UtcNow.AddYears(-MAX_AGE + 1)).Value);
        pet.UpdateColor(Color.Create("white").Value);
        await WriteDbContext.SaveChangesAsync();


        var query = new GetFilteredPetsWithPaginationQuery(
            PAGE,
            PAGE_SIZE,
            volunteer1.Id,
            pet.Name.Value,
            pet.PetClassification.SpeciesId,
            pet.PetClassification.BreedId,
            pet.Color.Value,
            pet.Address.City,
            pet.Address.House,
            pet.Address.Apartment,
            MAX_WEIGHT,
            MAX_HEIGHT,
            pet.OwnerPhoneNumber.Value,
            pet.IsCastrated.Value,
            MAX_AGE,
            pet.IsVaccinated.Value,
            pet.HelpStatus.Value.ToString(),
            "age",
            "asc");

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.TotalCount.Should().Be(1);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items[0].Name.Should().Be("chip");
    }
}