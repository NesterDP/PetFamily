using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder
            .Property(u => u.SocialNetworks)
            .CustomListJsonCollectionConverter(
                socialNetwork => new SocialNetworkDto(socialNetwork.Name, socialNetwork.Link),
                dto => SocialNetwork.Create(dto.Name, dto.Link).Value)
            .HasColumnName("social_networks");
        
        builder.ComplexProperty(u => u.FullName, fnb =>
        {
            fnb.Property(fn => fn.FirstName)
                .IsRequired(true)
                .HasMaxLength(DomainConstants.MAX_NAME_LENGTH)
                .HasColumnName("first_name");
            
            fnb.Property(fn => fn.LastName)
                .IsRequired(true)
                .HasMaxLength(DomainConstants.MAX_NAME_LENGTH)
                .HasColumnName("last_name");
            
            fnb.Property(fn => fn.Surname)
                .IsRequired(false)
                .HasMaxLength(DomainConstants.MAX_NAME_LENGTH)
                .HasColumnName("surname");
        });
        
        builder.Property(u => u.Photo)
            .IsRequired(false)
            .HasColumnName("photo");

        builder
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<IdentityUserRole<Guid>>();

        builder
            .HasOne(u => u.ParticipantAccount)
            .WithOne(p => p.User)
            .HasForeignKey<ParticipantAccount>(p => p.UserId)
            .IsRequired(false); 

    
        builder
            .HasOne(u => u.VolunteerAccount)
            .WithOne(v => v.User)
            .HasForeignKey<VolunteerAccount>(v => v.UserId)
            .IsRequired(false); 
        
        builder
            .HasOne(u => u.AdminAccount)
            .WithOne(a => a.User)
            .HasForeignKey<AdminAccount>(a => a.UserId)
            .IsRequired(false); 
    }
}