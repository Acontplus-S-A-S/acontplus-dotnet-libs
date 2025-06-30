namespace Acontplus.FactElect.Helpers;

public static class ResourceHelper
{
    public static Stream GetXsdStream(string xsdFileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.{xsdFileName.Replace("/", ".").Replace("\\", ".")}";
        return assembly.GetManifestResourceStream(resourceName) ??
               throw new FileNotFoundException($"Resource '{resourceName}' not found.");
    }
}