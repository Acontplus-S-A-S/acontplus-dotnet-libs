namespace Acontplus.Core.Configuration;

public sealed class CacheSettings
{
    public const string SectionName = "CacheSettings";

    public int SizeLimitMb { get; init; } = 100;
    public double CompactionPercentage { get; init; } = 0.25;
    public int ExpirationScanFrequencyMinutes { get; init; } = 5;
    public int DefaultExpirationMinutes { get; init; } = 30;

    public CacheProfileSettings Profiles { get; init; } = new();
}

public sealed class CacheProfileSettings
{
    public int ShortExpirationMinutes { get; init; } = 5;
    public int MediumExpirationMinutes { get; init; } = 30;
    public int LongExpirationMinutes { get; init; } = 120;
    public int VeryLongExpirationMinutes { get; init; } = 1440;
}
