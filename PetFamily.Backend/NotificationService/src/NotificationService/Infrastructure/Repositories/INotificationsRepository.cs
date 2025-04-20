using CSharpFunctionalExtensions;
using NotificationService.Core.CustomErrors;
using NotificationService.DataModels;
using NotificationService.DataModels.ValueObjects;

namespace NotificationService.Infrastructure.Repositories;

public interface INotificationsRepository
{
    Task<Guid> AddAsync(UserNotificationSettings settings, CancellationToken cancellationToken = default);
    
    Guid Save(UserNotificationSettings settings, CancellationToken cancellationToken = default);
    
    Guid Delete(UserNotificationSettings settings, CancellationToken cancellationToken = default);

    Task<Result<UserNotificationSettings, Error>> GetByIdAsync(
        UserNotificationSettingsId id,
        CancellationToken cancellationToken = default);
}