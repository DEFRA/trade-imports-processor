namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class BtmsOptions
{
    public const string SectionName = "Btms";

    public bool PublishToIpaffs { get; init; } = true;
}
