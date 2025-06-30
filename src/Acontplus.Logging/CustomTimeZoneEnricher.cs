namespace Acontplus.Logging;

public class CustomTimeZoneEnricher : ILogEventEnricher
{
    private readonly TimeZoneInfo _timeZone;

    public CustomTimeZoneEnricher(string timeZoneId)
    {
        try
        {
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            _timeZone = TimeZoneInfo.Utc; // Fallback to UTC
        }
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(logEvent.Timestamp.UtcDateTime, _timeZone);
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CustomTimestamp", localTime));
    }
}
