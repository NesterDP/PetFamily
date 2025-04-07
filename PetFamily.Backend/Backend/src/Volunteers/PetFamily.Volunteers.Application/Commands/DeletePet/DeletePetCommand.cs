using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Commands.DeletePet;

public record DeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;