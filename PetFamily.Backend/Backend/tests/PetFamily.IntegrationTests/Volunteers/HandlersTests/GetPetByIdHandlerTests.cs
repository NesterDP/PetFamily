using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Application.Queries.GetPetById;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class GetPetByIdHandlerTests : VolunteerTestsBase
{
    private readonly IQueryHandler<Result<PetDto, ErrorList>, GetPetByIdQuery> _sut;

    public GetPetByIdHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<Result<PetDto, ErrorList>, GetPetByIdQuery>>();
    }

    [Fact]
    public async Task GetPetById_Success_returns_info_about_non_soft_deleted_pet_with_correct_photo_order()
    {
        var PET_COUNT = 5;
        // arrange
        List<TransferDetail> transferDetails =
        [
            TransferDetail.Create("mir", "for transfers within country").Value,
            TransferDetail.Create("visa", "for transfers outside of country").Value
        ];
        
        List<Photo> photos =
        [
            Photo.Create(Guid.NewGuid(), Photo.AllowedTypes.First()).Value,
            Photo.Create(Guid.NewGuid(), true, Photo.AllowedTypes.First()).Value,
        ];
        
        var MAIN_PHOTO = photos[1].Id;

        var volunteer = await DataGenerator.SeedVolunteer(VolunteersWriteDbContext);
        var species = await DataGenerator.SeedSpecies(SpeciesWriteDbContext);
        var breed = await DataGenerator.SeedBreed(SpeciesWriteDbContext, species.Id);
        var pet = DataGenerator.CreatePet(species.Id, breed.Id, "1");
        pet.UpdatePhotos(photos);
        pet.UpdateTransferDetails(transferDetails);
        volunteer.AddPet(pet);
        await VolunteersWriteDbContext.SaveChangesAsync();
        var petSeeder = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var query = new GetPetByIdQuery(pet.Id);
        
        Factory.SetupSuccessGetFilesPresignedUrlsMock(photos.Select(p => p.Id.Value).ToList());

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(pet.Id);
        result.Value.OwnerId.Should().Be(volunteer.Id);
        result.Value.Name.Should().Be(pet.Name.Value);
        result.Value.Description.Should().Be(pet.Description.Value);
        result.Value.SpeciesId.Should().Be(species.Id);
        result.Value.BreedId.Should().Be(breed.Id);
        result.Value.Color.Should().Be(pet.Color.Value);
        result.Value.HealthInfo.Should().Be(pet.HealthInfo.Value);
        result.Value.City.Should().Be(pet.Address.City);
        result.Value.House.Should().Be(pet.Address.House);
        result.Value.Apartment.Should().Be(pet.Address.Apartment);
        result.Value.Weight.Should().Be(pet.Weight.Value);
        result.Value.Height.Should().Be(pet.Height.Value);
        result.Value.OwnerPhoneNumber.Should().Be(pet.OwnerPhoneNumber.Value);
        result.Value.IsCastrated.Should().Be(pet.IsCastrated.Value);
        result.Value.DateOfBirth.Year.Should().Be(pet.DateOfBirth.Value.Year);
        result.Value.IsVaccinated.Should().Be(pet.IsVaccinated.Value);
        result.Value.HelpStatus.Should().Be(pet.HelpStatus.Value.ToString());
        result.Value.CreationDate.Year.Should().Be(pet.CreationDate.Year);
        result.Value.Position.Should().Be(pet.Position.Value);
        result.Value.IsDeleted.Should().Be(pet.IsDeleted);
        
        result.Value.TransferDetails[0].Name.Should().Be(transferDetails[0].Name);
        result.Value.TransferDetails[0].Description.Should().Be(transferDetails[0].Description);
        result.Value.TransferDetails[1].Name.Should().Be(transferDetails[1].Name);
        result.Value.TransferDetails[1].Description.Should().Be(transferDetails[1].Description);

        // main photo should be the first one
        result.Value.Photos[0].Id.Should().Be(MAIN_PHOTO);
        result.Value.Photos[0].Main.Should().Be(true);
        result.Value.Photos[0].Url.Should().NotBeNull();
        result.Value.Photos[1].Id.Should().Be(photos[0].Id);
        result.Value.Photos[1].Main.Should().Be(false);
        result.Value.Photos[1].Url.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetPetById_failure_returns_error_for_soft_deleted_pet()
    {
        // arrange
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        volunteer.SoftDeletePet(pet);
        await VolunteersWriteDbContext.SaveChangesAsync();
        var query = new GetPetByIdQuery(pet.Id);
        
        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetPetById_returns_error_for_pet_that_does_not_exist()
    {
        // arrange
        var PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var query = new GetPetByIdQuery(Guid.NewGuid());
        
        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);
        
        // assert
        result.IsFailure.Should().BeTrue();
    }
}