using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Species.Domain.Entities;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;
using PetFamily.Volunteers.Infrastructure.DbContexts;

namespace PetFamily.IntegrationTests.General;

public static class DataGenerator
{
    public static Volunteer CreateVolunteer(string suffix = "", int exp = 1)
    {
        var volunteerId = VolunteerId.NewVolunteerId();
        var email = Email.Create($"test{suffix}@test.com").Value;
        var fullName = FullName
            .Create($"testFirstName{suffix}", $"testLastName{suffix}", $"testSurname{suffix}").Value;
        var description = Description.Create($"testDescription{suffix}").Value;
        var experience = Experience.Create(exp).Value;
        var phoneNumber = Phone.Create("1-2-333-44-55-66").Value;
        var socialNetworksList = new List<SocialNetwork>();
        var transferDetailsList = new List<TransferDetail>();

        var volunteer = new Volunteer(
            volunteerId,
            fullName,
            email,
            description,
            experience,
            phoneNumber,
            socialNetworksList,
            transferDetailsList);

        return volunteer;
    }

    public static Pet CreatePet(Guid speciesId, Guid breedId, string suffix = "")
    {
        var petId = PetId.NewPetId();
        var name = Name.Create($"testName{suffix}").Value;
        var description = Description.Create($"testDescription{suffix}").Value;
        var petClassification = PetClassification
            .Create(speciesId, breedId).Value;
        var color = Color.Create($"testColor{suffix}").Value;
        var healthInfo = HealthInfo.Create($"testHealthInfo{suffix}").Value;
        var address = Address
            .Create($"testCity{suffix}", $"testStreet{suffix}", $"testApartment{suffix}").Value;
        var weight = Weight.Create(1).Value;
        var height = Height.Create(1).Value;
        var phoneNumber = Phone.Create("1-2-333-44-55-66").Value;
        var isCastrated = IsCastrated.Create(false).Value;
        var dateOfBirth = DateOfBirth.Create(DateTime.UtcNow.AddYears(-2)).Value;
        var isVaccinated = IsVaccinated.Create(false).Value;
        var helpStatus = HelpStatus.Create(PetStatus.InSearchOfHome).Value;
        var transferDetailsList = new List<TransferDetail>();
        var photosList = new List<Photo>();

        var pet = new Pet(
            petId,
            name,
            description,
            petClassification,
            color,
            healthInfo,
            address,
            weight,
            height,
            phoneNumber,
            isCastrated,
            dateOfBirth,
            isVaccinated,
            helpStatus,
            transferDetailsList,
            photosList);

        return pet;
    }

    public static User CreateUser(string username, string email)
    {
        var user = new User
        {
            UserName = username,
            Email = email,
        };

        return user;
    }

    public static async Task<IdentityResult> SeedUserAsync(
        string username,
        string email,
        string password,
        UserManager<User> userManager)
    {
        var user = CreateUser(username, email);
        var result = await userManager.CreateAsync(user, password);

        return result;
    }

    public static Species.Domain.Entities.Species CreateSpecies(string suffix = "")
    {
        var speciesId = Guid.NewGuid();
        var name = Name.Create($"test specie{suffix}").Value;
        var species = new Species.Domain.Entities.Species(speciesId, name);

        return species;
    }

    public static Breed CreateBreed(string suffix = "")
    {
        var breedId = Guid.NewGuid();
        var name = Name.Create($"test breed{suffix}").Value;
        var breed = new Breed(breedId, name);

        return breed;
    }

    public static Volunteer CreateVolunteerWithPets(int count, Guid speciesId, Guid breedId)
    {
        var volunteer = CreateVolunteer();
        var pets = CreatePetList(count, speciesId, breedId);
        pets.ForEach(pet => volunteer.AddPet(pet));
        return volunteer;
    }

    public static List<Pet> CreatePetList(int count, Guid speciesId, Guid breedId)
    {
        var pets = Enumerable.Range(1, count).Select(_ => CreatePet(speciesId, breedId));
        return pets.ToList();
    }

    public static async Task<Volunteer> SeedVolunteer(WriteDbContext dbContext)
    {
        var volunteer = CreateVolunteer();
        dbContext.Volunteers.Add(volunteer);
        await dbContext.SaveChangesAsync();
        return volunteer;
    }
    
    public static async Task<List<Volunteer>> SeedVolunteers(WriteDbContext dbContext, int count)
    {
        List<Volunteer> volunteers = [];
        for (int i = 0; i < count; i++)
        {
            var volunteer = CreateVolunteer();
            dbContext.Volunteers.Add(volunteer);
            await dbContext.SaveChangesAsync();
            volunteers.Add(volunteer);
        }
        return volunteers;
    }

    public static async Task<Species.Domain.Entities.Species> SeedSpecies(
        Species.Infrastructure.DbContexts.WriteDbContext dbContext)
    {
        var species = CreateSpecies();
        dbContext.Species.Add(species);
        await dbContext.SaveChangesAsync();

        return species;
    }

    public static async Task<Breed> SeedBreed(
        Species.Infrastructure.DbContexts.WriteDbContext dbContext,
        Guid speciesId)
    {
        var species = await dbContext.Species.FirstOrDefaultAsync(s => s.Id == speciesId);

        var breed = CreateBreed();
        species!.AddBreed(breed);
        await dbContext.SaveChangesAsync();

        return breed;
    }

    public static async Task<Volunteer> SeedVolunteerWithPets(
        WriteDbContext dbContext,
        int count,
        Guid speciesId,
        Guid breedId)
    {
        var volunteer = CreateVolunteerWithPets(count, speciesId, breedId);
        dbContext.Volunteers.Add(volunteer);
        await dbContext.SaveChangesAsync();

        return volunteer;
    }

    public static async Task<Volunteer> SeedVolunteerWithPets(
        WriteDbContext dbContext,
        Species.Infrastructure.DbContexts.WriteDbContext speciesDbContext,
        int count)
    {
        var species = await SeedSpecies(speciesDbContext);
        var breed = await SeedBreed(speciesDbContext, species.Id);

        var volunteer = CreateVolunteerWithPets(count, species.Id, breed.Id);
        dbContext.Volunteers.Add(volunteer);
        await dbContext.SaveChangesAsync();

        return volunteer;
    }
}