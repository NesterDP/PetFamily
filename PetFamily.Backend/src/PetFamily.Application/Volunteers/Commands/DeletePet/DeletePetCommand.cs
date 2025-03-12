using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Volunteers.Commands.DeletePet;

public record DeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;