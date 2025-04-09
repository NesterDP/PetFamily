using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Core.Dto.Discussion;

namespace PetFamily.Discussions.Infrastructure.Configurations.Read;

public class MessageDtoConfiguration : IEntityTypeConfiguration<MessageDto>
{
    public void Configure(EntityTypeBuilder<MessageDto> builder)
    {
        builder.ToTable("messages");
        
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .IsRequired()
            .HasColumnName("id");
        
        builder.Property(m => m.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(m => m.Text)
            .IsRequired()
            .HasColumnName("text");
        
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
        
        builder.Property(m => m.IsEdited)
            .IsRequired()
            .HasColumnName("is_edited");
    }
}