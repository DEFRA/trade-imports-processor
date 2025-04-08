#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class FeedbackInformationAuthorityTypeEnumMapper
{
    public static IpaffsDataApi.FeedbackInformationAuthorityType? Map(FeedbackInformationAuthorityType? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            FeedbackInformationAuthorityType.Exitbip => IpaffsDataApi.FeedbackInformationAuthorityType.Exitbip,
            FeedbackInformationAuthorityType.Finalbip => IpaffsDataApi.FeedbackInformationAuthorityType.Finalbip,
            FeedbackInformationAuthorityType.Localvetunit => IpaffsDataApi
                .FeedbackInformationAuthorityType
                .Localvetunit,
            FeedbackInformationAuthorityType.Inspunit => IpaffsDataApi.FeedbackInformationAuthorityType.Inspunit,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
