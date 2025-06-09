using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;

public class ExcludePropertyJsonConverter<T>(params string[] propertyNames) : JsonConverter<T>
{
    private readonly HashSet<string> _propertiesToExclude = new(propertyNames, StringComparer.OrdinalIgnoreCase);

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var property in typeof(T).GetProperties())
        {
            if (_propertiesToExclude.Contains(property.Name))
                continue;

            var propertyValue = property.GetValue(value);

            var propertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;

            if (propertyValue != null)
            {
                writer.WritePropertyName(propertyName);
                JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
            }
        }

        writer.WriteEndObject();
    }
}
