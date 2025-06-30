using System.Reflection;

namespace Acontplus.Utilities.IO;

public static class DirectoryHelper
{
    public static string GetRuntimeDirectory()
    {
        // Retrieve the directory of the currently executing assembly
        var runtimeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

        // Throw an exception if the directory could not be determined
        if (string.IsNullOrEmpty(runtimeDirectory))
        {
            throw new InvalidOperationException("Unable to determine the runtime directory. Ensure the entry assembly is properly configured.");
        }

        return runtimeDirectory;
    }

}
