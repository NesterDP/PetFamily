using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Dto.Volunteer;

public class VolunteerDto
{
    public Guid Id { get; init; }
    public FullNameDto FullName { get; init; } = new(string.Empty, string.Empty, string.Empty);
    public string Email { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Experience { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public SocialNetworksDto SocialNetworks { get; init; } = new([]);
    public TransferDetailsDto TransferDetails { get; init; } = new([]);
}