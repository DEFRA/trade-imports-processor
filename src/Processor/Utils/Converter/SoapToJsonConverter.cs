using System.Xml;
using System.Xml.Linq;

namespace Defra.TradeImportsProcessor.Processor.Utils.Converter;

public static class SoapToJsonConverter
{
    public static string? Convert(XmlDocument soapDocument, string messageSubXPath)
    {
        var localNameXPath = SoapUtils.MakeLocalNameXPath(messageSubXPath);
        var xpath = $"/*[local-name()='Envelope']/*[local-name()='Body']/{localNameXPath}";

        var xmlMessage = soapDocument.DocumentElement?.SelectSingleNode(xpath)?.OuterXml;

        if (xmlMessage is null)
            return null;

        var xElement = XElement.Parse(xmlMessage);

        return XmlToJsonConverter.Convert(xElement);
    }
}
