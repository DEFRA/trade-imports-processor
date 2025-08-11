using System.Text.Json;

namespace Defra.TradeImportsProcessor.Processor.Utils.Converter;

public static class Json
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    };
}
