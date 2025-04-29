namespace NotificationService.Security.Authorization;

public static class PermissionExtensions
{
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        string permissionCode)
    {
        // 1. Добавляем PermissionAttribute в метаданные эндпоинта
        builder.WithMetadata(new PermissionAttribute(permissionCode));

        // 2. Включаем авторизацию с policyName = permissionCode
        // (PermissionPolicyProvider создаст политику на лету)
        return builder.RequireAuthorization(permissionCode);
    }
}