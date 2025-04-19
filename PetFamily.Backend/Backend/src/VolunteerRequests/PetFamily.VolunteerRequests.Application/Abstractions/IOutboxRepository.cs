namespace PetFamily.VolunteerRequests.Application.Abstractions;

public interface IOutboxRepository
{
    Task Add<T>(T message, CancellationToken cancellationToken);
}