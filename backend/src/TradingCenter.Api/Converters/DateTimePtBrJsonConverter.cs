using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradingCenter.Api.Converters;

public class DateTimePtBrJsonConverter : JsonConverter<DateTime>
{
    private static readonly string[] Formats = new[]
    {
        "dd/MM/yyyy",
        "dd/MM/yyyy HH:mm:ss",
        "dd-MM-yyyy",
        "yyyy-MM-dd",
        "yyyy-MM-ddTHH:mm:ss.FFFZ",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss"
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (DateTime.TryParseExact(str, Formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var exactResult))
                {
                    return DateTime.SpecifyKind(exactResult, DateTimeKind.Utc);
                }
                if (DateTime.TryParse(str, new CultureInfo("pt-BR"), DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsedResult))
                {
                    return DateTime.SpecifyKind(parsedResult, DateTimeKind.Utc);
                }
            }
        }
        return DateTime.SpecifyKind(reader.GetDateTime(), DateTimeKind.Utc);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
    }
}
