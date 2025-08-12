using System.Diagnostics;

namespace Acontplus.Persistence.PostgreSQL.Utilities;

public static class DiagnosticConfig
{
    public static readonly ActivitySource ActivitySource = new("Repository");
}
