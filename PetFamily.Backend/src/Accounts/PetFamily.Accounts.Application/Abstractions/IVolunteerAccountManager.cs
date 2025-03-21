using PetFamily.Accounts.Domain.DataModels;

namespace PetFamily.Accounts.Application.Abstractions;

public interface IVolunteerAccountManager
{
    public Task CreateVolunteerAccount(VolunteerAccount volunteerAccount);
}