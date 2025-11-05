using System.Reflection;

namespace Acontplus.Utilities.IO;

/// <summary>
/// Provides helper methods for working with application directories.
/// </summary>
public static class DirectoryHelper
{
    /// <summary>
    /// Gets the directory of the currently executing entry assembly at runtime.
    /// </summary>
    /// <returns>The absolute path to the runtime directory.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the runtime directory cannot be determined.</exception>
    public static string GetRuntimeDirectory()
    {
        // Retrieve the directory of the currently executing assembly
        var runtimeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

        // Throw an exception if the directory could not be determined
        return string.IsNullOrEmpty(runtimeDirectory)
            ? throw new InvalidOperationException("Unable to determine the runtime directory. Ensure the entry assembly is properly configured.")
            : runtimeDirectory;
    }

}
