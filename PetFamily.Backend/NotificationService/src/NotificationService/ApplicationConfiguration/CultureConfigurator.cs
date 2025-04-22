using System.Globalization;

namespace NotificationService.ApplicationConfiguration;

public static class CultureConfigurator
{
    public static void Configure()
    {
        var cultureInfo = new CultureInfo("ru-RU"); // Используем культуру, где точка — разделитель
        cultureInfo.NumberFormat.NumberDecimalSeparator = "."; // Устанавливаем разделитель
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}