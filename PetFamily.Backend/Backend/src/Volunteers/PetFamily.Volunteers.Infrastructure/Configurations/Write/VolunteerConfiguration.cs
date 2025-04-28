using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.Entities;

namespace PetFamily.Volunteers.Infrastructure.Configurations.Write;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id).HasConversion(
            id => id.Value,
            value => VolunteerId.Create(value));

        builder.ComplexProperty(
            v => v.FullName,
            fnb =>
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
                    .IsRequired(false)
                    .HasMaxLength(DomainConstants.MAX_NAME_LENGTH)
                    .HasColumnName("surname");
            });

        builder.ComplexProperty(
            v => v.Email,
            eb =>
            {
                eb.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_LOW_TEXT_LENGTH)
                    .HasColumnName("email");
            });

        builder.ComplexProperty(
            v => v.Experience,
            eb =>
            {
                eb.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("experience");
            });

        builder.ComplexProperty(
            v => v.PhoneNumber,
            pnb =>
            {
                pnb.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_PHONE_LENGTH)
                    .HasColumnName("phone_number");
            });

        builder.HasMany(v => v.AllOwnedPets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.AllOwnedPets).AutoInclude();

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");

        builder.Property(p => p.DeletionDate)
            .IsRequired(false)
            .HasColumnName("deletion_date");
    }
}