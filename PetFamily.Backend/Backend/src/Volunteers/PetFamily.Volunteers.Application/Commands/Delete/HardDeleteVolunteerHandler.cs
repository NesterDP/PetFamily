using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Volunteers.Application.Commands.Delete;

public class HardDeleteVolunteerHandler : ICommandHandler<Guid, DeleteVolunteerCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<HardDeleteVolunteerHandler> _logger;
    private readonly IValidator<DeleteVolunteerCommand> _validator;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;

    public HardDeleteVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<HardDeleteVolunteerHandler> logger,
        IValidator<DeleteVolunteerCommand> validator,
        IFileService fileService,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)] IUnitOfWork unitOfWork)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteVolunteerCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        
        var volunteerId = VolunteerId.Create(command.Id);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        foreach (var pet in volunteerResult.Value.AllOwnedPets)
        {
            var request = new DeleteFilesByIdsRequest(pet.PhotosList.Select(p => p.Id.Value).ToList());
        
            var deleteResult = await _fileService.DeleteFilesByIds(request, cancellationToken);
            if (deleteResult.IsFailure)
                return Errors.General.Failure(deleteResult.Error).ToErrorList();
        }
        
        _volunteersRepository.Delete(volunteerResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Volunteer was hard deleted, his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}