using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Core.Extensions;

namespace PetFamily.Accounts.Infrastructure.Configurations;

public class VolunteerAccountConfiguration : IEntityTypeConfiguration<VolunteerAccount>
{
    public void Configure(EntityTypeBuilder<VolunteerAccount> builder)
    {
        builder.ToTable("volunteer_accounts");
        
        builder
            .Property(v => v.TransferDetails)
            .CustomListJsonCollectionConverter(
                transferDetail => new TransferDetailDto(transferDetail.Name, transferDetail.Description),
                dto => new TransferDetailDto(dto.Name, dto.Description))
            .HasColumnName("transfer_details");
        
        builder
            .Property(v => v.Certificates)
            .CustomListJsonCollectionConverter(
                certificate => new CertificateDto(certificate.Name, certificate.GivenBy, certificate.YearOfAcquisition),
                dto => new CertificateDto(dto.Name, dto.GivenBy, dto.YearOfAcquisition))
            .HasColumnName("certificates");
        
        builder.Property(v => v.Experience)
            .HasColumnName("experience");
    }
}