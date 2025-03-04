using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Infrastructure.Extensions;

namespace PetFamily.Infrastructure.Configurations.Write;

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
                .HasMaxLength(DomainConstants.MAX_NAME_LENGTH)
                .HasColumnName("first_name");
            
            fnb.Property(pn => pn.LastName)
                .IsRequired()
                .HasMaxLength(DomainConstants.MAX_NAME_LENGTH)
                .HasColumnName("last_name");
            
            fnb.Property(pn => pn.Surname)
                .IsRequired()
                .HasMaxLength(DomainConstants.MAX_NAME_LENGTH)
                .HasColumnName("surname");
        });

        builder.ComplexProperty(v => v.Email, eb =>
        {
            eb.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(DomainConstants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("email");
        });
        
        builder.ComplexProperty(v => v.Description, db =>
        {
            db.Property(d => d.Value)
                .IsRequired()
                .HasMaxLength(DomainConstants.MAX_HIGH_TEXT_LENGTH)
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
                .HasMaxLength(DomainConstants.MAX_PHONE_LENGTH)
                .HasColumnName("phone_number");
        });
        
        builder.Property(v => v.SocialNetworksList)
            .Json1DeepLvlVoCollectionConverter(
                socialNetwork => new SocialNetworkDto(socialNetwork.Name, socialNetwork.Link),
                dto => SocialNetwork.Create(dto.Link, dto.Name).Value)
            .HasColumnName("social_networks");

        builder.Property(v => v.TransferDetailsList)
            .Json1DeepLvlVoCollectionConverter(
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