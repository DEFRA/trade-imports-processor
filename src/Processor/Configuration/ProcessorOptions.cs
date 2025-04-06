namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class CdpOptions
{
    [ConfigurationKeyName("CDP_HTTPS_PROXY")]
    public string? CdpHttpsProxy { get; set; }
}
