using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Domain.DataModels;

public class User : IdentityUser<Guid>
{

    private List<Role> _roles = [];
    
    public IReadOnlyList<Role>  Roles => _roles;

    private User() { } // ef core
    
    public string Photo { get; set; }
    
    public FullName FullName { get; set; } 
    
    public List<SocialNetwork> SocialNetworks = [];
    
    public ParticipantAccount? ParticipantAccount { get; set; } // navigation
    
    public VolunteerAccount? VolunteerAccount { get; set; } // navigation
    
    public AdminAccount? AdminAccount { get; set; } // navigation
    
    public static Result<User, Error> CreateAdmin(
        string userName,
        string email,
        FullName fullname,
        Role role)
    {
        if (role.Name != DataModels.AdminAccount.ADMIN)
            return Errors.General.ValueIsInvalid("role");
        
        var admin = new User()
        {
            UserName = userName,
            Email = email,
            FullName = fullname,
            _roles =  [role]
        };

        return admin;
    }
    
    public static Result<User, Error> CreateParticipant(
        string userName,
        string email,
        FullName fullname,
        Role role)
    {
        if (role.Name != DataModels.ParticipantAccount.PARTICIPANT)
            return Errors.General.ValueIsInvalid("role");
        
        var participant = new User()
        {
            UserName = userName,
            Email = email,
            FullName = fullname,
            _roles =  [role]
        };
        return participant;
    }
}