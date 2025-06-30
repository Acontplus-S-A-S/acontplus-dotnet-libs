namespace Acontplus.Utilities.Time;

public static class DateUtils
{
    public static DateTime GetEcuadorDate()
    {
        var currentTime = DateTime.Now;
        var ecuadorZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        var ecuadorTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentTime, ecuadorZone.Id);
        return ecuadorTime;
    }
}
