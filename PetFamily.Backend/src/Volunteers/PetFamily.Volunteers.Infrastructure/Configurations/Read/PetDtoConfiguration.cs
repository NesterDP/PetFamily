using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;

namespace PetFamily.Volunteers.Infrastructure.Configurations.Read;

public class PetDtoConfiguration : IEntityTypeConfiguration<PetDto>
{
    public void Configure(EntityTypeBuilder<PetDto> builder)
    {
        builder.ToTable("pets");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(v => v.IsDeleted)
            .HasColumnName("is_deleted");
        
        builder.Property(p => p.OwnerId).HasColumnName("volunteer_id");
        builder.Property(p => p.Height).HasColumnName("height_info");
        builder.Property(p => p.OwnerPhoneNumber).HasColumnName("owner_phone");
        builder.Property(p => p.Weight).HasColumnName("weight_info");
        builder.Property(p => p.HelpStatus)
            .HasConversion(
                status => 0,
                    status => ((PetStatus)status).ToString())
                .HasColumnName("help_status");

        builder.Property(v => v.TransferDetails)
            .HasConversion(
                transferDetail => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<TransferDetailDto[]>(json, JsonSerializerOptions.Default)!)
            .HasColumnName("transfer_details");
        
        builder.Property(v => v.Photos)
            .HasConversion(
                transferDetail => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<PhotoDto[]>(json, JsonSerializerOptions.Default)!)
            .HasColumnName("photos");
    }
}