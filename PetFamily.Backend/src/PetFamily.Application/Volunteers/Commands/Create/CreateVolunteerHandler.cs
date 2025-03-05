using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Commands.Create;

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
        

        var volunteerId = VolunteerId.NewVolunteerId();

        var fullNameCreateResult = FullName.Create(
            command.VolunteerCommandDto.FullName.FirstName,
            command.VolunteerCommandDto.FullName.LastName,
            command.VolunteerCommandDto.FullName.Surname);

        var emailCreateResult = Email.Create(command.VolunteerCommandDto.Email);

        var descriptionCreateResult = Description.Create(command.VolunteerCommandDto.Description);

        var experienceCreateResult = Experience.Create(command.VolunteerCommandDto.Experience);

        var phoneNumberCreateResult = Phone.Create(command.VolunteerCommandDto.PhoneNumber);

        List<SocialNetwork> socialNetworksList = [];
        foreach (var socialNetwork in command.SocialNetworksDto)
        {
            var result = SocialNetwork.Create(socialNetwork.Name, socialNetwork.Link);
            socialNetworksList.Add(result.Value);
        }

        List<TransferDetail> transferDetailsList = [];
        foreach (var transferDetail in command.TransferDetailsDto)
        {
            var result = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
            transferDetailsList.Add(result.Value);
        }
        

        var volunteer = Volunteer.Create(
            volunteerId,
            fullNameCreateResult.Value,
            emailCreateResult.Value,
            descriptionCreateResult.Value,
            experienceCreateResult.Value,
            phoneNumberCreateResult.Value,
            socialNetworksList,
            transferDetailsList);

        await _volunteersRepository.AddAsync(volunteer.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created volunteer with ID = {id}", volunteerId.Value);
        
        return volunteer.Value.Id.Value;
    }
}