using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Linq;

namespace Defra.TradeImportsProcessor.Processor.Utils.Converter;

[SuppressMessage("SonarLint", "S5332", Justification = "The HTTP web links are XML namespaces so cannot change")]
public static class SoapUtils
{
    private static readonly XNamespace SoapNs = "http://www.w3.org/2003/05/soap-envelope";
    private static readonly XNamespace OasNs =
        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
    private static readonly XAttribute RoleAttribute = new(SoapNs + "role", "system");

    private static readonly XNamespace AlvsCommonRootNs = "http://uk.gov.hmrc.ITSW2.ws";
    private static readonly XAttribute AlvsSoapNsAttribute = new(XNamespace.Xmlns + "NS1", SoapNs);
    private static readonly XAttribute AlvsSecurityNsAttribute = new(XNamespace.Xmlns + "NS2", OasNs);
    private static readonly XAttribute AlvsCommonRootNsAttribute = new(XNamespace.Xmlns + "NS3", AlvsCommonRootNs);

    public static readonly string DecisionNotificationRootAttribute =
        "http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification";

    public static XElement AddSoapEnvelope(XElement rootElement, string? username = null, string? password = null)
    {
        return GetAlvsToCdsSoapEnvelope(rootElement, username, password);
    }

    public static XmlDocument ToXmlDocument(string soapContent)
    {
        var decodedSoap = soapContent.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(decodedSoap);

        return xmlDoc;
    }

    public static string? GetProperty(XmlDocument soapDocument, string propertyXPath)
    {
        var localNameXPath = MakeLocalNameXPath(propertyXPath);
        var xpath = $"//{localNameXPath}";

        return soapDocument.SelectSingleNode(xpath)?.InnerXml;
    }

    public static string MakeLocalNameXPath(string messageSubXPath)
    {
        return string.Join('/', messageSubXPath.Trim('/').Split('/').Select(element => $"*[local-name()='{element}']"));
    }

    private static XElement GetAlvsToCdsSoapEnvelope(
        XElement rootElement,
        string? username = null,
        string? password = null
    )
    {
        XNamespace rootNs = DecisionNotificationRootAttribute;

        return new XElement(
            SoapNs + "Envelope",
            AlvsSoapNsAttribute,
            new XElement(
                SoapNs + "Header",
                new XElement(
                    OasNs + "Security",
                    AlvsSecurityNsAttribute,
                    RoleAttribute,
                    new XElement(
                        OasNs + "UsernameToken",
                        new XElement(OasNs + "Username", username ?? "ibmtest"),
                        new XElement(OasNs + "Password", password ?? "password")
                    )
                )
            ),
            new XElement(
                SoapNs + "Body",
                new XElement(
                    AlvsCommonRootNs + rootElement.Name.LocalName,
                    AlvsCommonRootNsAttribute,
                    AddNamespace(rootElement, rootNs).ToString().Replace("\n", "").Replace("  ", "")
                )
            )
        );
    }

    private static XElement AddNamespace(XElement element, XNamespace rootNs)
    {
        element.Name = rootNs + element.Name.LocalName;
        foreach (var child in element.Elements())
            AddNamespace(child, rootNs);

        return element;
    }
}
