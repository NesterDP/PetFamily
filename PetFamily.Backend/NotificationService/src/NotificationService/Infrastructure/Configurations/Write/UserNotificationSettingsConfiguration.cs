using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.DataModels;
using NotificationService.DataModels.ValueObjects;

namespace NotificationService.Infrastructure.Configurations.Write;

public class UserNotificationSettingsConfiguration : IEntityTypeConfiguration<UserNotificationSettings>
{
    public void Configure(EntityTypeBuilder<UserNotificationSettings> builder)
    {
        builder.ToTable("users_notification_settings");
        
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => UserNotificationSettingsId.Create(value))
            .HasColumnName("id");
        
        builder.Property(u => u.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .HasColumnName("user_id");

        builder.Property(u => u.SendToEmail)
            .IsRequired()
            .HasColumnName("send_to_email");
        
        builder.Property(u => u.SendToTelegram)
            .IsRequired()
            .HasColumnName("send_to_telegram");
        
        builder.Property(u => u.SendToWebsite)
            .IsRequired()
            .HasColumnName("send_to_website");
    }
}