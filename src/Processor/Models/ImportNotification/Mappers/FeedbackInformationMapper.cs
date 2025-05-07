using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class FeedbackInformationMapper
{
    public static IpaffsDataApi.FeedbackInformation Map(FeedbackInformation? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.FeedbackInformation
        {
            AuthorityType = from.AuthorityType,
            ConsignmentArrival = from.ConsignmentArrival,
            ConsignmentConformity = from.ConsignmentConformity,
            ConsignmentNoArrivalReason = from.ConsignmentNoArrivalReason,
            DestructionDate = from.DestructionDate,
        };

        return to;
    }
}
