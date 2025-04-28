using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Application.Commands.Create;

public class CreateVolunteerHandler : ICommandHandler<Guid, CreateVolunteerCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<CreateVolunteerHandler> _logger;
    private readonly IValidator<CreateVolunteerCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVolunteerHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<CreateVolunteerHandler> logger,
        IValidator<CreateVolunteerCommand> validator,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateVolunteerCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.CreateVolunteerDto.UserId);

        var fullNameCreateResult = FullName.Create(
            command.CreateVolunteerDto.FullName.FirstName,
            command.CreateVolunteerDto.FullName.LastName,
            command.CreateVolunteerDto.FullName.Surname);

        var emailCreateResult = Email.Create(command.CreateVolunteerDto.Email);

        var experienceCreateResult = Experience.Create(command.CreateVolunteerDto.Experience);

        var phoneNumberCreateResult = Phone.Create(command.CreateVolunteerDto.PhoneNumber);

        var volunteer = Volunteer.Create(
            volunteerId,
            fullNameCreateResult.Value,
            emailCreateResult.Value,
            experienceCreateResult.Value,
            phoneNumberCreateResult.Value);

        await _volunteersRepository.AddAsync(volunteer.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created volunteer with ID = {id}", volunteerId.Value);

        return volunteer.Value.Id.Value;
    }
}