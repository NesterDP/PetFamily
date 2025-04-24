using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Domain.DataModels;

public class User : IdentityUser<Guid>
{
    private List<Role> _roles = [];

    public IReadOnlyList<Role> Roles => _roles;

    public Avatar Avatar { get; set; } = null!;

    public FullName FullName { get; init; } = null!;

    public List<SocialNetwork> SocialNetworks { get; init; } = [];

    public ParticipantAccount? ParticipantAccount { get; init; } // navigation

    public VolunteerAccount? VolunteerAccount { get; init; } // navigation

    public AdminAccount? AdminAccount { get; init; } // navigation

    public static Result<User, Error> CreateAdmin(
        string userName,
        string email,
        FullName fullname,
        Role role)
    {
        if (role.Name != DomainConstants.ADMIN)
            return Errors.General.ValueIsInvalid("role");

        var admin = new User()
        {
            UserName = userName, Email = email, FullName = fullname, _roles = [role],
        };

        return admin;
    }

    public static Result<User, Error> CreateParticipant(
        string userName,
        string email,
        FullName fullname,
        Role role)
    {
        if (role.Name != DomainConstants.PARTICIPANT)
            return Errors.General.ValueIsInvalid("role");

        var participant = new User()
        {
            UserName = userName, Email = email, FullName = fullname, _roles = [role],
        };
        return participant;
    }

    private User() { } // ef core
}