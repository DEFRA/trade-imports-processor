namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class DevEndpointsOptions
{
    public const string SectionName = "DevEndpoints";

    public bool Enabled { get; init; } = false;
}
