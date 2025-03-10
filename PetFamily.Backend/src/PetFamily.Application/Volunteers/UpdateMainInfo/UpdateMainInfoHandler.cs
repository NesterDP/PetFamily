using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdateMainInfoHandler> _logger;
    private readonly IValidator<UpdateMainInfoCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMainInfoHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<UpdateMainInfoHandler> logger,
        IValidator<UpdateMainInfoCommand> validator ,
        IUnitOfWork unitOfWork)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateMainInfoCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        
        var volunteerId = VolunteerId.Create(command.Id);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var fullName = FullName.Create(
            command.FullNameDto.FirstName,
            command.FullNameDto.LastName,
            command.FullNameDto.Surname).Value;
        var email = Email.Create(command.Email).Value;
        var description = Description.Create(command.Description).Value;
        var experience = Experience.Create(command.Experience).Value;
        var phone = Phone.Create(command.PhoneNumber).Value;
        volunteerResult.Value.UpdateMainInfo(fullName, email, description, experience, phone);

        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Volunteer was updated (main info), his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}