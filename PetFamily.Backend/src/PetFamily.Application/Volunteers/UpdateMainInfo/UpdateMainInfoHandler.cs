using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoHandler
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UpdateMainInfoHandler> _logger;

    public UpdateMainInfoHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<UpdateMainInfoHandler> logger)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
    }


    public async Task<Result<Guid, Error>> HandleAsync(
        UpdateMainInfoRequest request,
        CancellationToken cancellationToken)
    {
        var volunteerId = VolunteerId.Create(request.Id);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;

        var fullName = FullName.Create(
            request.Dto.FullName.FirstName,
            request.Dto.FullName.LastName,
            request.Dto.FullName.Surname).Value;
        var email = Email.Create(request.Dto.Email).Value;
        var description = Description.Create(request.Dto.Description).Value;
        var experience = Experience.Create(request.Dto.Experience).Value;
        var phone = Phone.Create(request.Dto.PhoneNumber).Value;
        volunteerResult.Value.UpdateMainInfo(fullName, email, description, experience, phone);

        var result = await _volunteersRepository.SaveAsync(volunteerResult.Value, cancellationToken);

        _logger.LogInformation("Volunteer was updated (main info), his ID = {ID}", volunteerId.Value);

        return volunteerId.Value;
    }
}