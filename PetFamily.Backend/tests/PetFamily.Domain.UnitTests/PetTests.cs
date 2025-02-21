using FluentAssertions;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Domain.UnitTests;

public class PetTests
{

    [Fact]
    public void Add_Pet_To_Empty_Pet_List_Return_Success_Result()
    {
        // Arrange
        var volunteer = CreateVolunteer();
        var pet = CreatePet();

        // Act
        var result = volunteer.AddPet(pet);

        // Assert
        var addedPetResult = volunteer.GetPetById(pet.Id);

        Assert.True(result.IsSuccess);
        Assert.True(addedPetResult.IsSuccess);
        Assert.True(addedPetResult.Value.Id == pet.Id);
        Assert.True(addedPetResult.Value.Position == Position.First);
    }

    [Fact]
    public void Add_Pet_To_Non_Empty_Pet_List_Return_Success_Result()
    {
        // Arrange
        const int petsCount = 5;
        var volunteer = CreateVolunteerWithPets(petsCount);
        var positionOfLastAdded = Position.Create(petsCount + 1).Value;
        var pet = CreatePet();
        
        // Act
        var result = volunteer.AddPet(pet);

        // Assert
        var addedPetResult = volunteer.GetPetById(pet.Id);

        Assert.True(result.IsSuccess);
        Assert.True(addedPetResult.IsSuccess);
        Assert.True(addedPetResult.Value.Id == pet.Id);
        Assert.True(addedPetResult.Value.Position == positionOfLastAdded);
    }

    [Fact]
    public void Move_Pet_Should_Not_Move_When_Pet_Is_Already_At_New_Position()
    {
        // Arrange
        const int petsCount = 5;
        var volunteer = CreateVolunteerWithPets(petsCount);
        
        var positionTo = Position.Create(2).Value;
        
        var firstPet = volunteer.AllOwnedPets[0];
        var secondPet = volunteer.AllOwnedPets[1];
        var thirdPet = volunteer.AllOwnedPets[2];
        var fourthPet = volunteer.AllOwnedPets[3];
        var fifthPet = volunteer.AllOwnedPets[4];
        

        // Act
        var result = volunteer.MovePet(secondPet, positionTo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(1);
        secondPet.Position.Value.Should().Be(2);
        thirdPet.Position.Value.Should().Be(3);
        fourthPet.Position.Value.Should().Be(4);
        fifthPet.Position.Value.Should().Be(5);
    }
    
    
    
    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Forward_When_New_Position_Is_Lower()
    {
        // Arrange
        const int petsCount = 5;
        var volunteer = CreateVolunteerWithPets(petsCount);
        
        var positionTo = Position.Create(2).Value;
        
        var firstPet = volunteer.AllOwnedPets[0];
        var secondPet = volunteer.AllOwnedPets[1];
        var thirdPet = volunteer.AllOwnedPets[2];
        var fourthPet = volunteer.AllOwnedPets[3];
        var fifthPet = volunteer.AllOwnedPets[4];
        

        // Act
        var result = volunteer.MovePet(fourthPet, positionTo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(1);
        secondPet.Position.Value.Should().Be(3);
        thirdPet.Position.Value.Should().Be(4);
        fourthPet.Position.Value.Should().Be(2);
        fifthPet.Position.Value.Should().Be(5);
    }
    
    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Backward_When_New_Position_Is_Higher()
    {
        // Arrange
        const int petsCount = 5;
        var volunteer = CreateVolunteerWithPets(petsCount);
        
        var positionTo = Position.Create(4).Value;
        
        var firstPet = volunteer.AllOwnedPets[0];
        var secondPet = volunteer.AllOwnedPets[1];
        var thirdPet = volunteer.AllOwnedPets[2];
        var fourthPet = volunteer.AllOwnedPets[3];
        var fifthPet = volunteer.AllOwnedPets[4];
        

        // Act
        var result = volunteer.MovePet(secondPet, positionTo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(1);
        secondPet.Position.Value.Should().Be(4);
        thirdPet.Position.Value.Should().Be(2);
        fourthPet.Position.Value.Should().Be(3);
        fifthPet.Position.Value.Should().Be(5);
    }
    
    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Forward_When_New_Position_Is_First()
    {
        // Arrange
        const int petsCount = 5;
        var volunteer = CreateVolunteerWithPets(petsCount);
        
        var positionTo = Position.Create(1).Value;
        
        var firstPet = volunteer.AllOwnedPets[0];
        var secondPet = volunteer.AllOwnedPets[1];
        var thirdPet = volunteer.AllOwnedPets[2];
        var fourthPet = volunteer.AllOwnedPets[3];
        var fifthPet = volunteer.AllOwnedPets[4];
        

        // Act
        var result = volunteer.MovePet(fifthPet, positionTo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(2);
        secondPet.Position.Value.Should().Be(3);
        thirdPet.Position.Value.Should().Be(4);
        fourthPet.Position.Value.Should().Be(5);
        fifthPet.Position.Value.Should().Be(1);
    }
    
    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Backward_When_New_Position_Is_Last()
    {
        // Arrange
        const int petsCount = 5;
        var volunteer = CreateVolunteerWithPets(petsCount);
        
        var positionTo = Position.Create(5).Value;
        
        var firstPet = volunteer.AllOwnedPets[0];
        var secondPet = volunteer.AllOwnedPets[1];
        var thirdPet = volunteer.AllOwnedPets[2];
        var fourthPet = volunteer.AllOwnedPets[3];
        var fifthPet = volunteer.AllOwnedPets[4];
        

        // Act
        var result = volunteer.MovePet(firstPet, positionTo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(5);
        secondPet.Position.Value.Should().Be(1);
        thirdPet.Position.Value.Should().Be(2);
        fourthPet.Position.Value.Should().Be(3);
        fifthPet.Position.Value.Should().Be(4);
    }
    
    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Backward_When_New_Position_Is_Beyond_Higher_Bound()
    {
        // Arrange
        const int petsCount = 5;
        var volunteer = CreateVolunteerWithPets(petsCount);
        
        var positionTo = Position.Create(100).Value;
        
        var firstPet = volunteer.AllOwnedPets[0];
        var secondPet = volunteer.AllOwnedPets[1];
        var thirdPet = volunteer.AllOwnedPets[2];
        var fourthPet = volunteer.AllOwnedPets[3];
        var fifthPet = volunteer.AllOwnedPets[4];
        

        // Act
        var result = volunteer.MovePet(firstPet, positionTo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Value.Should().Be(5);
        secondPet.Position.Value.Should().Be(1);
        thirdPet.Position.Value.Should().Be(2);
        fourthPet.Position.Value.Should().Be(3);
        fifthPet.Position.Value.Should().Be(4);
    }
    
      private static Volunteer CreateVolunteer()
    {
        var volunteerId = VolunteerId.NewVolunteerId();
        var email = Email.Create("test@test.com").Value;
        var fullName = FullName
            .Create("testFirstName", "testLastName", "testSurname").Value;
        var description = Description.Create("testDescription").Value;
        var experience = Experience.Create(1).Value;
        var phoneNumber = Phone.Create("1-2-333-44-55-66").Value;
        var socialNetworkList = SocialNetworkList.Create(new List<SocialNetwork>()).Value;
        var transferDetailList = TransferDetailList.Create(new List<TransferDetail>()).Value;

        var volunteer = new Volunteer(
            volunteerId,
            fullName,
            email,
            description,
            experience,
            phoneNumber,
            socialNetworkList,
            transferDetailList);

        return volunteer;
    }

    private static Pet CreatePet()
    {
        
        var petId = PetId.NewPetId();
        var name = Name.Create("testName").Value;
        var description = Description.Create("testDescription").Value;
        var petClassification = PetClassification
            .Create(Guid.NewGuid(), Guid.NewGuid()).Value;
        var color = Color.Create("testColor").Value;
        var healthInfo = HealthInfo.Create("testHealthInfo").Value;
        var address = Address.Create("testCity", "testStreet", "testApartment").Value;
        var weight = Weight.Create(1).Value;
        var height = Height.Create(1).Value;
        var phoneNumber = Phone.Create("1-2-333-44-55-66").Value;
        var isCastrated = IsCastrated.Create(false).Value;
        var dateOfBirth = DateOfBirth.Create(DateTime.Now.AddYears(-2)).Value;
        var isVaccinated = IsVaccinated.Create(true).Value;
        var helpStatus = HelpStatus.Create(PetStatus.NeedHelp).Value;
        var transferDetailList = TransferDetailList
            .Create(new List<TransferDetail>()).Value;

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
            transferDetailList);

        return pet;
    }

    private static List<Pet> CreatePetList(int count)
    {
        var pets = Enumerable.Range(1, count).Select(_ => CreatePet());
        return pets.ToList();
    }
    
    private static Volunteer CreateVolunteerWithPets(int count)
    {
        var volunteer = CreateVolunteer();
        var pets = Enumerable.Range(1, count).Select(_ => CreatePet());
        foreach (var pet in pets)
            volunteer.AddPet(pet);
        return volunteer;
    }
}