#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectionCheckTypeEnumMapper
{
    public static IpaffsDataApi.InspectionCheckType? Map(InspectionCheckType? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            InspectionCheckType.PhsiDocument => IpaffsDataApi.InspectionCheckType.PhsiDocument,
            InspectionCheckType.PhsiIdentity => IpaffsDataApi.InspectionCheckType.PhsiIdentity,
            InspectionCheckType.PhsiPhysical => IpaffsDataApi.InspectionCheckType.PhsiPhysical,
            InspectionCheckType.Hmi => IpaffsDataApi.InspectionCheckType.Hmi,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
