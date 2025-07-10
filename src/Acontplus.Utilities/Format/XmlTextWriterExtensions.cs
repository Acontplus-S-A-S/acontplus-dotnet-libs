namespace Acontplus.Utilities.Format;

/// <summary>
/// Provides extension methods for writing XML elements with optional values.
/// </summary>
public static class XmlTextWriterExtensions
{
    /// <summary>
    /// Writes an XML element with the specified name and value, writing an empty string if the value is null.
    /// </summary>
    /// <param name="writer">The <see cref="XmlTextWriter"/> to write to.</param>
    /// <param name="elementName">The name of the XML element.</param>
    /// <param name="value">The value to write inside the element, or null for an empty element.</param>
    public static void WriteElement(this XmlTextWriter writer, string elementName, string? value)
    {
        writer.WriteStartElement(elementName);
        writer.WriteString(value ?? string.Empty);
        writer.WriteEndElement();
    }
}
