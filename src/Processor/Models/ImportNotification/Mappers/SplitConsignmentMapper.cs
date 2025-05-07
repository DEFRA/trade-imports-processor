using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class SplitConsignmentMapper
{
    public static IpaffsDataApi.SplitConsignment Map(SplitConsignment? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.SplitConsignment
        {
            ValidReferenceNumber = from.ValidReferenceNumber,
            RejectedReferenceNumber = from.RejectedReferenceNumber,
        };

        return to;
    }
}
