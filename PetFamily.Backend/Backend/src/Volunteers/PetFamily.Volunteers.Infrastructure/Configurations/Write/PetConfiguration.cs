using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Extensions.EfCoreFluentApiExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;

namespace PetFamily.Volunteers.Infrastructure.Configurations.Write;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasConversion(
            id => id.Value,
            value => PetId.Create(value));

        builder.ComplexProperty(
            p => p.Name,
            nb =>
            {
                nb.Property(n => n.Value)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_NAME_LENGTH)
                    .HasColumnName("name");
            });

        builder.ComplexProperty(
            p => p.Description,
            db =>
            {
                db.Property(d => d.Value)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_HIGH_TEXT_LENGTH)
                    .HasColumnName("description");
            });

        builder.ComplexProperty(
            p => p.PetClassification,
            pcb =>
            {
                pcb.Property(c => c.BreedId)
                    .IsRequired()
                    .HasColumnName("breed_id");

                pcb.Property(c => c.SpeciesId)
                    .IsRequired()
                    .HasColumnName("species_id");
            });

        builder.ComplexProperty(
            p => p.Color,
            cb =>
            {
                cb.Property(c => c.Value)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_LOW_TEXT_LENGTH)
                    .HasColumnName("color");
            });

        builder.ComplexProperty(
            p => p.HealthInfo,
            hb =>
            {
                hb.Property(h => h.Value)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_MEDIUM_TEXT_LENGTH)
                    .HasColumnName("health_info");
            });

        builder.ComplexProperty(
            p => p.Address,
            ab =>
            {
                ab.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_LOGISTIC_UNIT_LENGTH)
                    .HasColumnName("city");

                ab.Property(a => a.House)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_LOGISTIC_UNIT_LENGTH)
                    .HasColumnName("house");

                ab.Property(a => a.Apartment)
                    .IsRequired(false)
                    .HasMaxLength(DomainConstants.MAX_LOGISTIC_UNIT_LENGTH)
                    .HasColumnName("apartment");
            });

        builder.ComplexProperty(
            p => p.Weight,
            wb =>
            {
                wb.Property(a => a.Value)
                    .IsRequired()
                    .HasColumnName("weight_info");
            });

        builder.ComplexProperty(
            p => p.Height,
            hb =>
            {
                hb.Property(a => a.Value)
                    .IsRequired()
                    .HasColumnName("height_info");
            });

        builder.ComplexProperty(
            p => p.OwnerPhoneNumber,
            pb =>
            {
                pb.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(DomainConstants.MAX_PHONE_LENGTH)
                    .HasColumnName("owner_phone");
            });

        builder.ComplexProperty(
            p => p.IsCastrated,
            icb =>
            {
                icb.Property(ic => ic.Value)
                    .IsRequired()
                    .HasColumnName("is_castrated");
            });

        builder.ComplexProperty(
            p => p.DateOfBirth,
            dobb =>
            {
                dobb.Property(dob => dob.Value)
                    .IsRequired()
                    .HasColumnName("date_of_birth");
            });

        builder.ComplexProperty(
            p => p.IsVaccinated,
            ivb =>
            {
                ivb.Property(iv => iv.Value)
                    .IsRequired()
                    .HasColumnName("is_vaccinated");
            });

        builder.ComplexProperty(
            p => p.HelpStatus,
            hsb =>
            {
                hsb.Property(hs => hs.Value)
                    .IsRequired()
                    .HasConversion(
                        status => (int)status,
                        status => (PetStatus)status)
                    .HasColumnName("help_status");
            });

        builder.Property(p => p.TransferDetailsList)
            .CustomJsonCollectionConverter(
                transferDetail => new TransferDetailDto(transferDetail.Name, transferDetail.Description),
                dto => TransferDetail.Create(dto.Name, dto.Description).Value)
            .HasColumnName("transfer_details");

        builder.Property(p => p.PhotosList)
            .CustomJsonCollectionConverter(
                photo => new PhotoDto(photo.Id.Value, photo.Main),
                dto => Photo.Create(FileId.Create(dto.Id), dto.Main, Photo.AllowedTypes.First()).Value)
            .HasColumnName("photos");

        builder.Property(p => p.CreationDate)
            .IsRequired()
            .HasDefaultValue(DateTime.MinValue)
            .HasColumnName("creation_date");

        builder.ComplexProperty(
            p => p.Position,
            snb =>
            {
                snb.Property(sn => sn.Value)
                    .IsRequired()
                    .HasColumnName("position");
            });

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");

        builder.Property(p => p.DeletionDate)
            .IsRequired(false)
            .HasColumnName("deletion_date");
    }
}