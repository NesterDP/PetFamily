using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Pets.Queries.GetPetById;
using PetFamily.Application.Volunteers.Commands.UpdateTransferDetails;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.SharedVO;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class GetPetByIdHandlerTests : VolunteerTestsBase
{
    private readonly IQueryHandler<PetDto, GetPetByIdQuery> _sut;

    public GetPetByIdHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<PetDto, GetPetByIdQuery>>();
    }

    [Fact]
    public async Task GetPetById_returns_info_about_non_soft_deleted_pet_with_correct_photo_order()
    {
        var PET_COUNT = 5;
        // arrange
        List<TransferDetail> transferDetails =
        [
            TransferDetail.Create("mir", "for transfers within country").Value,
            TransferDetail.Create("visa", "for transfers outside of country").Value
        ];
        
        const string MAIN_PHOTO = "new_photo_2.jpg";
        List<Photo> photos =
        [
            new Photo(FilePath.Create("new_photo_1.jpg").Value),
            new Photo(FilePath.Create(MAIN_PHOTO).Value, true)
        ];

        var volunteer = await DataGenerator.SeedVolunteer(WriteDbContext);
        var species = await DataGenerator.SeedSpecies(WriteDbContext);
        var breed = await DataGenerator.SeedBreed(WriteDbContext, species.Id);
        var pet = DataGenerator.CreatePet(species.Id, breed.Id, "1");
        pet.UpdatePhotos(photos);
        pet.UpdateTransferDetails(transferDetails);
        volunteer.AddPet(pet);
        await WriteDbContext.SaveChangesAsync();
        var petSeeder = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var query = new GetPetByIdQuery(pet.Id);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.Should().NotBeNull();
        result.Id.Should().Be(pet.Id);
        result.OwnerId.Should().Be(volunteer.Id);
        result.Name.Should().Be(pet.Name.Value);
        result.Description.Should().Be(pet.Description.Value);
        result.SpeciesId.Should().Be(species.Id);
        result.BreedId.Should().Be(breed.Id);
        result.Color.Should().Be(pet.Color.Value);
        result.HealthInfo.Should().Be(pet.HealthInfo.Value);
        result.City.Should().Be(pet.Address.City);
        result.House.Should().Be(pet.Address.House);
        result.Apartment.Should().Be(pet.Address.Apartment);
        result.Weight.Should().Be(pet.Weight.Value);
        result.Height.Should().Be(pet.Height.Value);
        result.OwnerPhoneNumber.Should().Be(pet.OwnerPhoneNumber.Value);
        result.IsCastrated.Should().Be(pet.IsCastrated.Value);
        result.DateOfBirth.Year.Should().Be(pet.DateOfBirth.Value.Year);
        result.IsVaccinated.Should().Be(pet.IsVaccinated.Value);
        result.HelpStatus.Should().Be(pet.HelpStatus.Value.ToString());
        result.CreationDate.Year.Should().Be(pet.CreationDate.Year);
        result.Position.Should().Be(pet.Position.Value);
        result.IsDeleted.Should().Be(pet._isDeleted);
        
        result.TransferDetails[0].Name.Should().Be(transferDetails[0].Name);
        result.TransferDetails[0].Description.Should().Be(transferDetails[0].Description);
        result.TransferDetails[1].Name.Should().Be(transferDetails[1].Name);
        result.TransferDetails[1].Description.Should().Be(transferDetails[1].Description);

        // main photo should be the first one
        result.Photos[0].PathToStorage.Should().Be(MAIN_PHOTO);
        result.Photos[0].Main.Should().Be(true);
        result.Photos[1].PathToStorage.Should().Be(photos[0].PathToStorage.Path);
        result.Photos[1].Main.Should().Be(photos[0].Main);
    }
    
    [Fact]
    public async Task GetPetById_returns_null_for_soft_deleted_pet()
    {
        // arrange
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        volunteer.SoftDeletePet(pet);
        await WriteDbContext.SaveChangesAsync();
        var query = new GetPetByIdQuery(pet.Id);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetPetById_returns_null_for_pet_that_does_not_exist()
    {
        // arrange
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(WriteDbContext, PET_COUNT);
        var query = new GetPetByIdQuery(Guid.NewGuid());

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.Should().BeNull();
    }
}