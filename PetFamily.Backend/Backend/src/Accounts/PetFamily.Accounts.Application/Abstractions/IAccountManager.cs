using PetFamily.Accounts.Domain.DataModels;

namespace PetFamily.Accounts.Application.Abstractions;

public interface IAccountManager
{
    public Task CreateParticipantAccount(ParticipantAccount participantAccount);
    
    public Task CreateVolunteerAccount(VolunteerAccount volunteerAccount);
    
    public Task CreateAdminAccount(AdminAccount adminAccount);
}