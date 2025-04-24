using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NotificationService.Core.CustomErrors;
using NotificationService.DataModels;
using NotificationService.DataModels.ValueObjects;
using NotificationService.Infrastructure.DbContexts;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationsRepository : INotificationsRepository
{
    private readonly WriteDbContext _context;

    public NotificationsRepository(WriteDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Guid> AddAsync(UserNotificationSettings settings, CancellationToken cancellationToken = default)
    {
        await _context.UsersNotificationSettings.AddAsync(settings, cancellationToken);
        return settings.Id;
    }

    public Guid Save(UserNotificationSettings settings, CancellationToken cancellationToken = default)
    {
        _context.UsersNotificationSettings.Attach(settings);
        return settings.Id;
    }

    public Guid Delete(UserNotificationSettings settings, CancellationToken cancellationToken = default)
    {
        _context.UsersNotificationSettings.Remove(settings);
        return settings.Id;
    }

    public async Task<Result<UserNotificationSettings, Error>> GetByIdAsync(
        UserNotificationSettingsId id,
        CancellationToken cancellationToken = default)
    {
        var settings = await _context.UsersNotificationSettings
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (settings == null)
            return Errors.General.ValueNotFound();

        return settings;
    }

    public async Task<Result<UserNotificationSettings, Error>> GetByUserIdAsync(
        UserId id,
        CancellationToken cancellationToken = default)
    {
        var settings = await _context.UsersNotificationSettings
            .FirstOrDefaultAsync(v => v.UserId == id, cancellationToken);

        if (settings == null)
            return Errors.General.ValueNotFound();

        return settings;
    }
}