namespace Acontplus.Utilities.IO;

public static class EnvironmentHelper
{
    public static EnvironmentEnums GetEnvironment()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        ArgumentNullException.ThrowIfNull(environmentName);

        return (EnvironmentEnums)Enum.Parse(typeof(EnvironmentEnums), environmentName);
    }
}
