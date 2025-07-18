using System.Diagnostics;

namespace Acontplus.Persistence.Postgres.Utilities;

public static class DiagnosticConfig
{
    public static readonly ActivitySource ActivitySource = new("Repository");
}
