namespace Acontplus.Utilities.Format;

public static class XmlTextWriterExtensions
{
    public static void WriteElement(this XmlTextWriter writer, string elementName, string? value)
    {
        writer.WriteStartElement(elementName);
        writer.WriteString(value ?? string.Empty);
        writer.WriteEndElement();
    }
}
