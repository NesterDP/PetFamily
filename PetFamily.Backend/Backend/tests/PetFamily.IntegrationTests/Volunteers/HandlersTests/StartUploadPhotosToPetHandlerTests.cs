using AutoFixture;
using FileService.Contracts.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.IntegrationTests.General;
using PetFamily.IntegrationTests.Volunteers.Heritage;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;
using PetFamily.Volunteers.Application.Commands.StartUploadPhotosToPet;

namespace PetFamily.IntegrationTests.Volunteers.HandlersTests;

public class StartUploadPhotosToPetHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<StartMultipartUploadResponse, StartUploadPhotosToPetCommand> _sut;

    public StartUploadPhotosToPetHandlerTests(VolunteerTestsWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<ICommandHandler<StartMultipartUploadResponse, StartUploadPhotosToPetCommand>>();
    }

    [Fact]
    public async Task StartUploadPhotos_success_should_generate_starting_multipart_upload_data()
    {
        // arrange
        int PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];

        var startUploadFileDto1 = new StartUploadFileDto("fileName1", "image/jpg", 123);
        var startUploadFileDto2 = new StartUploadFileDto("fileName2", "image/jpg", 123);
        
        var command = new StartUploadPhotosToPetCommand(
            volunteer.Id,
            pet.Id,
            [startUploadFileDto1, startUploadFileDto2]);
        
        Factory.SetupSuccessStartMultipartMock(command.FileInfos.Count());

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MultipartStartInfos.Should().HaveCount(command.FileInfos.Count);
    }

    [Fact]
    public async Task StartUploadPhotos_failure_should_return_error()
    {
        // arrange
        int PET_COUNT = 5;
        var volunteer = await DataGenerator.SeedVolunteerWithPets(VolunteersWriteDbContext, SpeciesWriteDbContext, PET_COUNT);
        var pet = volunteer.AllOwnedPets[0];
        var startUploadFileDto = new StartUploadFileDto("fileName1", "image/gif", 123);

        var command = new StartUploadPhotosToPetCommand(
            volunteer.Id,
            pet.Id,
            [startUploadFileDto]);
        
        Factory.SetupFailureStartMultipartMock();

        // act
        var result = await _sut.HandleAsync(command);

        // assert
        result.IsFailure.Should().BeTrue();
    }
}