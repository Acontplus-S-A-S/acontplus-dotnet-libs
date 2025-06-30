using System.Diagnostics;

namespace Acontplus.Persistence.SqlServer.Diagnostics;

public static class DiagnosticConfig
{
    public static readonly ActivitySource ActivitySource = new("Repository");
}
