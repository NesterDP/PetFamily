using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.Configurations;

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
                .HasMaxLength(Constants.MAX_NAME_LENGTH)
                .HasColumnName("first_name");
            
            fnb.Property(pn => pn.LastName)
                .IsRequired()
                .HasMaxLength(Constants.MAX_NAME_LENGTH)
                .HasColumnName("last_name");
            
            fnb.Property(pn => pn.Surname)
                .IsRequired()
                .HasMaxLength(Constants.MAX_NAME_LENGTH)
                .HasColumnName("surname");
        });

        builder.ComplexProperty(v => v.Email, eb =>
        {
            eb.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
                .HasColumnName("email");
        });
        
        builder.ComplexProperty(v => v.Description, db =>
        {
            db.Property(d => d.Value)
                .IsRequired()
                .HasMaxLength(Constants.MAX_HIGH_TEXT_LENGTH)
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
                .HasMaxLength(Constants.MAX_PHONE_LENGTH)
                .HasColumnName("phone_number");
        });

        builder.OwnsOne(v => v.SocialNetworkList, slb =>
        {
            slb.ToJson();

            slb.OwnsMany(snl => snl.SocialNetworks, snb =>
            {
                snb.Property(sn => sn.Name)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_NAME_LENGTH);
                snb.Property(sn => sn.Link)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
            });
        });
        
        builder.OwnsOne(v => v.TransferDetailList, tdlb =>
        {
            tdlb.ToJson();

            tdlb.OwnsMany(tdl => tdl.TransferDetails, tdb =>
            {
                tdb.Property(td => td.Name)
                    .IsRequired();
                tdb.Property(td => td.Description)
                    .IsRequired();
            });
        });

        builder.HasMany(v => v.AllOwnedPets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .IsRequired(false);
    }
}