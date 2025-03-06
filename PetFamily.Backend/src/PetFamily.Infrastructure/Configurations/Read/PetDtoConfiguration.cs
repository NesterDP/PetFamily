using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Infrastructure.Configurations.Read;

public class PetDtoConfiguration : IEntityTypeConfiguration<PetDto>
{
    public void Configure(EntityTypeBuilder<PetDto> builder)
    {
        builder.ToTable("pets");
        
        builder.HasKey(p => p.Id);
        
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
                json => JsonSerializer.Deserialize<TransferDetailDto[]>(json, JsonSerializerOptions.Default)!);
        
        builder.Property(v => v.Photos)
            .HasConversion(
                transferDetail => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<PhotoDto[]>(json, JsonSerializerOptions.Default)!);
    }
}