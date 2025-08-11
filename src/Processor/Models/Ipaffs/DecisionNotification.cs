namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class IpaffsDecisionNotification
{
    public required object serviceHeader { get; set; }
    public required object header { get; set; }
    public required object[] items { get; set; }
}
