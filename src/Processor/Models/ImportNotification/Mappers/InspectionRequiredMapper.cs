#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectionRequiredEnumMapper
{
    public static IpaffsDataApi.InspectionRequired? Map(InspectionRequired? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            InspectionRequired.Required => IpaffsDataApi.InspectionRequired.Required,
            InspectionRequired.Inconclusive => IpaffsDataApi.InspectionRequired.Inconclusive,
            InspectionRequired.NotRequired => IpaffsDataApi.InspectionRequired.NotRequired,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
