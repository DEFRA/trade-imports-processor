using System.Xml.Linq;

namespace Defra.TradeImportsProcessor.Processor.Utils.Converter;

public static class XDcoumentExtensions
{
    public static string ToStringWithDeclaration(this XDocument xDocument, XNamespace rootNs) =>
        $"{xDocument.Declaration}{xDocument.ToString(SaveOptions.DisableFormatting)}".Replace(
            "\"" + rootNs + "\"",
            "&quot;" + rootNs + "&quot;"
        );
}
