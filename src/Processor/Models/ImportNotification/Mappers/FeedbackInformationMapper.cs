#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class FeedbackInformationMapper
{
    public static IpaffsDataApi.FeedbackInformation Map(FeedbackInformation? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.FeedbackInformation();
        to.AuthorityType = FeedbackInformationAuthorityTypeEnumMapper.Map(from?.AuthorityType);
        to.ConsignmentArrival = from?.ConsignmentArrival;
        to.ConsignmentConformity = from?.ConsignmentConformity;
        to.ConsignmentNoArrivalReason = from?.ConsignmentNoArrivalReason;
        to.DestructionDate = from?.DestructionDate;
        return to;
    }
}
