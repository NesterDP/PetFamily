using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;
using FileInfo = PetFamily.Core.Files.FilesData.FileInfo;

namespace PetFamily.Volunteers.Application.Commands.DeletePet;

public class HardDeletePetHandler : ICommandHandler<Guid, DeletePetCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<HardDeletePetHandler> _logger;
    private readonly IValidator<DeletePetCommand> _validator;
    private readonly FileHttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;

    public HardDeletePetHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<HardDeletePetHandler> logger,
        IValidator<DeletePetCommand> validator,
        FileHttpClient httpClient,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeletePetCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.VolunteerId);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var pet = volunteerResult.Value.AllOwnedPets.FirstOrDefault(p => p.Id == command.PetId);
        if (pet == null)
            return Errors.General.ValueNotFound(command.PetId).ToErrorList();

        volunteerResult.Value.HardDeletePet(pet);
        
        // межсерверное взаимодействие, удаление данных из Mongo и S3 хранилища
        var request = new DeleteFilesByIdsRequest(pet.PhotosList.Select(p => p.Id.Value).ToList());
        
        var result = await _httpClient.DeleteFilesByIds(request, cancellationToken);
        if (result.IsFailure)
            return Errors.General.Failure(result.Error).ToErrorList();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet was hard deleted, his ID = {ID}", pet.Id);

        return pet.Id.Value;
    }
}