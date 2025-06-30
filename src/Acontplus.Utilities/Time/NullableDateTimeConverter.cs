using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.Utilities.Time;

/// <summary>
/// Converter for nullable DateTime values
/// </summary>
public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly string _dateFormat;

    public NullableDateTimeConverter(string dateFormat = "yyyy-MM-dd")
    {
        _dateFormat = dateFormat;
    }

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str))
                return null;
            if (DateTime.TryParse(str, out var date))
                return date;
        }
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        throw new JsonException("Invalid date format.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString(_dateFormat));
        else
            writer.WriteNullValue();
    }
}
