using System.Diagnostics;

namespace Acontplus.Persistence.SqlServer.Utilities;

public static class DiagnosticConfig
{
    public static readonly ActivitySource ActivitySource = new("Repository");
}
