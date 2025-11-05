namespace Acontplus.Persistence.SqlServer.Utilities;

public static class TimeZoneHelper
{
    // Zona horaria de Ecuador (ECT - Ecuador Time)
    private static readonly TimeZoneInfo EcuadorTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); // UTC-5

    // Zona horaria del servidor (configurable)
    private static readonly TimeZoneInfo ServerTimeZone = TimeZoneInfo.Local;

    /// <summary>
    /// OPCIÓN 1 (RECOMENDADA): Convertir UTC a zona horaria específica
    /// </summary>
    public static DateTime ToEcuadorTime(this DateTime utcDateTime)
    {
        return utcDateTime.Kind != DateTimeKind.Utc
            ? throw new ArgumentException("DateTime must be UTC", nameof(utcDateTime))
            : TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, EcuadorTimeZone);
    }

    public static DateTime? ToEcuadorTime(this DateTime? utcDateTime)
    {
        return utcDateTime?.ToEcuadorTime();
    }

    public static DateTime ToServerTime(this DateTime utcDateTime)
    {
        return utcDateTime.Kind != DateTimeKind.Utc
            ? throw new ArgumentException("DateTime must be UTC", nameof(utcDateTime))
            : TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ServerTimeZone);
    }

    /// <summary>
    /// Convertir de zona horaria local a UTC (para guardar en BD)
    /// </summary>
    public static DateTime FromEcuadorTimeToUtc(DateTime ecuadorDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(ecuadorDateTime, EcuadorTimeZone);
    }

    /// <summary>
    /// Obtener la hora actual en Ecuador
    /// </summary>
    public static DateTime NowInEcuador => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EcuadorTimeZone);

    /// <summary>
    /// Obtener la hora actual del servidor
    /// </summary>
    public static DateTime NowInServer => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ServerTimeZone);
}
