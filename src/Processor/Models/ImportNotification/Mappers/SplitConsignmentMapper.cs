using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class SplitConsignmentMapper
{
    public static IpaffsDataApi.SplitConsignment Map(SplitConsignment? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.SplitConsignment();
        to.ValidReferenceNumber = from?.ValidReferenceNumber;
        to.RejectedReferenceNumber = from?.RejectedReferenceNumber;
        return to;
    }
}
