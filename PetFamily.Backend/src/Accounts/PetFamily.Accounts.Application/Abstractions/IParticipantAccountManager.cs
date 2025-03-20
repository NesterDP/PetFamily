using PetFamily.Accounts.Domain.DataModels;

namespace PetFamily.Accounts.Application.Abstractions;

public interface IParticipantAccountManager
{
    public Task CreateParticipantAccount(ParticipantAccount participantAccount);
}