namespace Acontplus.FactElect.Interfaces.Services;

public interface IAtsXmlService
{
    /// <summary>
    /// Creates the ATS XML document.
    /// </summary>
    /// <param name="atsData">The data required to generate the ATS XML.</param>
    /// <returns>A byte array representing the generated XML.</returns>
    Task<byte[]> CreateAtsXmlAsync(AtsData atsData);
}