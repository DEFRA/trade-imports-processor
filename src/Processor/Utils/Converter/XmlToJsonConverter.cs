using System.Text.Json;
using System.Xml.Linq;

namespace Defra.TradeImportsProcessor.Processor.Utils.Converter;

public static class XmlToJsonConverter
{
    private static readonly KnownArray[] s_knownArrays =
    [
        new() { ItemName = "Item", ArrayName = "Items" },
        new() { ItemName = "Document", ArrayName = "Documents" },
        new() { ItemName = "Check", ArrayName = "Checks" },
        new() { ItemName = "Error", ArrayName = "Errors" },
    ];

    private static readonly string[] s_knownNumbers =
    [
        "EntryVersionNumber",
        "PreviousVersionNumber",
        "DecisionNumber",
        "ItemNumber",
        "ItemNetMass",
        "ItemSupplementaryUnits",
        "ItemThirdQuantity",
        "DocumentQuantity",
    ];

    public static string Convert(XContainer xContainer)
    {
        var jsonObject = new Dictionary<string, object>();
        ConvertElementToDictionary(xContainer, ref jsonObject);

        return JsonSerializer.Serialize(jsonObject, Json.SerializerOptions);
    }

    private static void ConvertElementToDictionary(XContainer xElement, ref Dictionary<string, object> parent)
    {
        foreach (var child in xElement.Elements())
        {
            if (child.HasElements)
            {
                HandleComplexElement(child, ref parent);
            }
            else
            {
                parent[child.Name.LocalName] = ConvertValue(child)!;
            }
        }
    }

    private static void HandleComplexElement(XElement child, ref Dictionary<string, object> parent)
    {
        var childObject = new Dictionary<string, object>();
        var elementName = child.Name.LocalName;

        var arrayName = GetArrayName(elementName);

        if (arrayName is not null)
        {
            AddToArray(parent, arrayName, childObject);
        }
        else
        {
            parent[elementName] = childObject;
        }

        ConvertElementToDictionary(child, ref childObject);
    }

    private static string? GetArrayName(string elementName)
    {
        return s_knownArrays.SingleOrDefault(x => x.ItemName == elementName)?.ArrayName;
    }

    private static void AddToArray(
        Dictionary<string, object> parent,
        string arrayName,
        Dictionary<string, object> childObject
    )
    {
        if (!parent.TryGetValue(arrayName, out var value))
        {
            parent[arrayName] = new List<Dictionary<string, object>> { childObject };
        }
        else if (value is List<Dictionary<string, object>> list)
        {
            list.Add(childObject);
        }
    }

    private static object? ConvertValue(XElement element)
    {
        if (element.IsEmpty)
            return null;
        if (bool.TryParse(element.Value, out var boolResult))
            return boolResult;
        if (int.TryParse(element.Value, out var intResult))
            return ConvertNumber(element, intResult);
        if (long.TryParse(element.Value, out var longResult))
            return ConvertNumber(element, longResult);
        if (decimal.TryParse(element.Value, out var decimalResult))
            return ConvertNumber(element, decimalResult);
        return element.Value;
    }

    private static object? ConvertNumber(XElement element, object? result)
    {
        return s_knownNumbers.Contains(element.Name.LocalName, StringComparer.InvariantCultureIgnoreCase)
            ? result
            : element.Value;
    }

    private sealed class KnownArray
    {
        public required string ArrayName { get; init; }
        public required string ItemName { get; init; }
    }
}
