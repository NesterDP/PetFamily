using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Infrastructure.Configurations.Write;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");
        
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.Id).
            HasConversion(
                id => id.Value,
                value => VolunteerId.Create(value));

        builder.ComplexProperty(v => v.FullName, fnb =>
        {
            fnb.Property(pn => pn.FirstName)
                .IsRequired()
                .HasMaxLength(SharedConstants.MAX_NAME_LENGTH)
                .HasColumnName("first_name");
            
            fnb.Property(pn => pn.LastName)
                .IsRequired()
                .HasMaxLength(SharedConstants.MAX_NAME_LENGTH)
                .HasColumnName("last_name");
            
            fnb.Property(pn => pn.Surname)
                .IsRequired()
                .HasMaxLength(SharedConstants.MAX_NAME_LENGTH)
                .HasColumnName("surname");
        });

        builder.ComplexProperty(v => v.Email, eb =>
        {
            eb.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(SharedConstants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("email");
        });
        
        builder.ComplexProperty(v => v.Description, db =>
        {
            db.Property(d => d.Value)
                .IsRequired()
                .HasMaxLength(SharedConstants.MAX_HIGH_TEXT_LENGTH)
                .HasColumnName("description");
        });
        
        builder.ComplexProperty(v => v.Experience, eb =>
        {
            eb.Property(e => e.Value)
                .IsRequired()
                .HasColumnName("experience");
        });
        
        builder.ComplexProperty(v => v.PhoneNumber, pnb =>
        {
            pnb.Property(p => p.Value)
                .IsRequired()
                .HasMaxLength(SharedConstants.MAX_PHONE_LENGTH)
                .HasColumnName("phone_number");
        });
        
        builder.Property(v => v.SocialNetworksList)
            .CustomJsonCollectionConverter(
                socialNetwork => new SocialNetworkDto(socialNetwork.Name, socialNetwork.Link),
                dto => SocialNetwork.Create(dto.Link, dto.Name).Value)
            .HasColumnName("social_networks");

        builder.Property(v => v.TransferDetailsList)
            .CustomJsonCollectionConverter(
                transferDetails => new TransferDetailDto(transferDetails.Name, transferDetails.Description),
                dto => TransferDetail.Create(dto.Name, dto.Description).Value)
            .HasColumnName("transfer_details");

        builder.HasMany(v => v.AllOwnedPets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.AllOwnedPets).AutoInclude();

        builder.Property<bool>("_isDeleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("is_deleted");
    }
}