using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Application.Queries.GetFilteredPetsWithPagination;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class GetFilteredPetsWithPaginationHandlerTests : VolunteerTestsBase
{
    private readonly GetFilteredPetsWithPaginationHandler _sut;

    public GetFilteredPetsWithPaginationHandlerTests(VolunteerTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<GetFilteredPetsWithPaginationHandler>();
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_all_pets()
    {
        // arrange
        const int PETS_COUNT = 16;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;

        await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT / 2);
        await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT / 2);

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Value.TotalCount.Should().Be(PETS_COUNT);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_only_filtered_pets()
    {
        // arrange
        const int PETS_COUNT = 16;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;

        var volunteer1 =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT / 2);
        await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT / 2);

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE, volunteer1.Id);
        var volunteer1PetsHash = volunteer1.AllOwnedPets.Select(p => p.Id.Value).ToHashSet();

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Value.TotalCount.Should().Be(8);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.Items.Select(i => i.Id).ToHashSet().Should().BeEquivalentTo(volunteer1PetsHash);
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_pets_with_photos_being_ordered_by_main_goes_first()
    {
        // arrange
        List<Photo> photos1 =
        [
            Photo.Create(Guid.NewGuid(), Photo.AllowedTypes.First()).Value,
            Photo.Create(Guid.NewGuid(), true, Photo.AllowedTypes.First()).Value,
        ];

        List<Photo> photos2 =
        [
            Photo.Create(Guid.NewGuid(), true, Photo.AllowedTypes.First()).Value,
            Photo.Create(Guid.NewGuid(), Photo.AllowedTypes.First()).Value
        ];

        const int PETS_COUNT = 16;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;
        var volunteer1 =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT / 2);
        var volunteer2 =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT / 2);
        volunteer1.AllOwnedPets[2].UpdatePhotos(photos1);
        volunteer2.AllOwnedPets[4].UpdatePhotos(photos2);

        await VolunteersWriteDbContext.SaveChangesAsync();

        var photosIds = photos1.Select(p => p.Id.Value).Union(photos2.Select(p => p.Id.Value))
            .ToList();

        Factory.SetupSuccessGetFilesPresignedUrlsMock(photosIds);

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Value.TotalCount.Should().Be(PETS_COUNT);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();

        var resultPhotosIds = new List<Guid>();
        foreach (var pet in result.Value.Items)
        {
            if (pet.Photos.Length != 0)
            {
                // main photo should always be first
                pet.Photos[0].Main.Should().BeTrue();

                // every photo should get its url
                foreach (var photo in pet.Photos)
                {
                    photo.Url.Should().NotBeNull();
                    resultPhotosIds.Add(photo.Id);
                }
            }
        }

        // each received photo should be unique
        resultPhotosIds.ToHashSet().Should().BeEquivalentTo(photosIds.ToHashSet());
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_pets_sorted_by_name_ascending_order()
    {
        // arrange
        const int PETS_COUNT = 3;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;
        var volunteer1 =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT);
        volunteer1.AllOwnedPets[0].UpdateName(Name.Create("barks").Value);
        volunteer1.AllOwnedPets[1].UpdateName(Name.Create("clap").Value);
        volunteer1.AllOwnedPets[2].UpdateName(Name.Create("angel").Value);
        await VolunteersWriteDbContext.SaveChangesAsync();

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE, SortBy: "name", SortDirection: "asc");

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Value.TotalCount.Should().Be(PETS_COUNT);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.Items[0].Name.Should().Be("angel");
        result.Value.Items[1].Name.Should().Be("barks");
        result.Value.Items[2].Name.Should().Be("clap");
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_pets_sorted_by_name_descending_order()
    {
        // arrange
        const int PETS_COUNT = 3;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;
        var volunteer1 =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT);
        volunteer1.AllOwnedPets[0].UpdateName(Name.Create("barks").Value);
        volunteer1.AllOwnedPets[1].UpdateName(Name.Create("clap").Value);
        volunteer1.AllOwnedPets[2].UpdateName(Name.Create("angel").Value);
        await VolunteersWriteDbContext.SaveChangesAsync();

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE, SortBy: "name", SortDirection: "desc");

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Value.TotalCount.Should().Be(PETS_COUNT);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.Items[0].Name.Should().Be("clap");
        result.Value.Items[1].Name.Should().Be("barks");
        result.Value.Items[2].Name.Should().Be("angel");
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_only_1_pet_the_rest_are_soft_deleted()
    {
        // arrange
        const int PETS_COUNT = 3;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;
        var volunteer1 =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT);
        volunteer1.AllOwnedPets[0].Delete();
        volunteer1.AllOwnedPets[1].Delete();
        volunteer1.AllOwnedPets[2].UpdateName(Name.Create("angel").Value);
        await VolunteersWriteDbContext.SaveChangesAsync();

        var query = new GetFilteredPetsWithPaginationQuery(PAGE, PAGE_SIZE);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.Value.TotalCount.Should().Be(1);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.Items[0].Name.Should().Be("angel");
    }

    [Fact]
    public async Task GetFilteredPetsWithPagination_should_return_only_1_pet_that_passed_all_possible_filters()
    {
        // arrange
        const int PETS_COUNT = 16;
        const int PAGE_SIZE = 16;
        const int PAGE = 1;
        const int MAX_AGE = 5;
        const int MAX_WEIGHT = 30;
        const int MAX_HEIGHT = 30;
        var volunteer1 =
            await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT);

        await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PETS_COUNT);

        var pet = volunteer1.AllOwnedPets[0];
        pet.UpdateName(Name.Create("chip").Value);
        pet.UpdateDateOfBirth(DateOfBirth.Create(DateTime.UtcNow.AddYears(-MAX_AGE + 1)).Value);
        pet.UpdateColor(Color.Create("white").Value);
        await VolunteersWriteDbContext.SaveChangesAsync();

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
        result.Value.TotalCount.Should().Be(1);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.Items[0].Name.Should().Be("chip");
    }
}