namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class BtmsOptions
{
    public const string SectionName = "Btms";

    public OperatingMode OperatingMode { get; init; } = OperatingMode.Default;
}
