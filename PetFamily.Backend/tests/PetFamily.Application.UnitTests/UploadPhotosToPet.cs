using System.Data;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Application.Database;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.FilesProvider;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Application.Volunteers;
using PetFamily.Application.Volunteers.UploadPhotosToPet;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.UnitTests;

public class UploadPhotosToPet
{
    [Fact]
    public async Task Handle_Should_Upload_Photos_to_Pet()
    {
        // arrange
        var ct = new CancellationTokenSource().Token;
        
        var logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<UploadPhotosToPetHandler>();

        var volunteer = CreateVolunteerWithPets(1);

        var stream = new MemoryStream();
        var fileName = "test.jpg";
        var uploadFileDto = new UploadFileDto(stream, fileName);
        List<UploadFileDto> files = [uploadFileDto, uploadFileDto];

        var command = new UploadPhotosToPetCommand(
            volunteer.Id.Value,
            volunteer.AllOwnedPets[0].Id.Value,
            files);

        var fileProviderMock = new Mock<IFilesProvider>();

        List<FilePath> filePaths =
        [
            FilePath.Create(fileName).Value,
            FilePath.Create(fileName).Value
        ];

        fileProviderMock
            .Setup(v => v.UploadFiles(It.IsAny<List<FileData>>(), ct))
            .ReturnsAsync(filePaths);
         // .ReturnsAsync(Result.Success<IReadOnlyList<FilePath>, Error>(filePaths));
        
        var volunteerRepositoryMock = new Mock<IVolunteersRepository>();

        volunteerRepositoryMock
            .Setup(v => v.GetByIdAsync(volunteer.Id, ct))
            .ReturnsAsync(volunteer);
        //  .Returns(Task.FromResult<Result<Volunteer, Error>>(Result.Success<Volunteer, Error>(volunteer)));
        
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(ct))
            .Returns(Task.CompletedTask);
        
        /*
        var loggerMock = new Mock<ILogger<UploadPhotosToPetHandler>>();
        loggerMock
            .Setup(l => l.LogInformation("Successfully uploaded photos to pet with ID"));*/
        
        
        var validatorMock = new Mock<IValidator<UploadPhotosToPetCommand>>();

        validatorMock.Setup(v => v.ValidateAsync(command, ct))
            .ReturnsAsync(new ValidationResult());
            
        var handler = new UploadPhotosToPetHandler(
            validatorMock.Object,
            volunteerRepositoryMock.Object,
            logger,
            fileProviderMock.Object,
            unitOfWorkMock.Object);
        
        // act
        var result = await handler.HandleAsync(command, ct);

        // assert
        volunteer.AllOwnedPets.First().PhotosList.Photos.Should().HaveCount(2);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(volunteer.AllOwnedPets[0].Id.Value);
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
        var socialNetworksList = SocialNetworksList.Create(new List<SocialNetwork>()).Value;
        var transferDetailsList = TransferDetailsList.Create(new List<TransferDetail>()).Value;

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
        var transferDetailsList = TransferDetailsList.Create(new List<TransferDetail>()).Value;
        var photosList = PhotosList.Create(new List<Photo>()).Value;

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

    private static Volunteer CreateVolunteerWithPets(int count)
    {
        var volunteer = CreateVolunteer();
        var pets = Enumerable.Range(1, count).Select(_ => CreatePet());
        foreach (var pet in pets)
            volunteer.AddPet(pet);
        return volunteer;
    }
}