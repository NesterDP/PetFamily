using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Volunteers.Infrastructure.Configurations.Read;

public class VolunteerDtoConfiguration : IEntityTypeConfiguration<VolunteerDto>
{
    public void Configure(EntityTypeBuilder<VolunteerDto> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.IsDeleted)
            .HasColumnName("is_deleted");
        
        builder.Property(v => v.SocialNetworks)
            .HasConversion(
                socialNetworks => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<SocialNetworkDto[]>(json, JsonSerializerOptions.Default)!)
            .HasColumnName("social_networks");
        
        builder.Property(v => v.TransferDetails)
            .HasConversion(
                transferDetails => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<TransferDetailDto[]>(json, JsonSerializerOptions.Default)!)
            .HasColumnName("transfer_details");

        builder.HasMany<PetDto>(v => v.Pets)
            .WithOne()
            .HasForeignKey(p => p.Id)
            .IsRequired(false);
        
        //builder.HasQueryFilter(v => v.)
    }
}