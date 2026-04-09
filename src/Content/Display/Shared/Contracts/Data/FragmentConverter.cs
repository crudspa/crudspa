using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class FragmentConverter : JsonConverter<String?>
{
    public override String? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
            return reader.GetString();

        using var document = JsonDocument.ParseValue(ref reader);
        return document.RootElement.GetRawText();
    }

    public override void Write(Utf8JsonWriter writer, String? value, JsonSerializerOptions options)
    {
        if (value.HasNothing())
        {
            writer.WriteNullValue();
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(value);
            document.RootElement.WriteTo(writer);
        }
        catch (JsonException)
        {
            writer.WriteStringValue(value);
        }
    }
}