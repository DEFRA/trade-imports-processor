#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionNotAcceptableActionQuarantineImposedReasonEnumMapper
{
    public static IpaffsDataApi.DecisionNotAcceptableActionQuarantineImposedReason? Map(
        DecisionNotAcceptableActionQuarantineImposedReason? from
    )
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            DecisionNotAcceptableActionQuarantineImposedReason.ContaminatedProducts => IpaffsDataApi
                .DecisionNotAcceptableActionQuarantineImposedReason
                .ContaminatedProducts,
            DecisionNotAcceptableActionQuarantineImposedReason.InterceptedPart => IpaffsDataApi
                .DecisionNotAcceptableActionQuarantineImposedReason
                .InterceptedPart,
            DecisionNotAcceptableActionQuarantineImposedReason.PackagingMaterial => IpaffsDataApi
                .DecisionNotAcceptableActionQuarantineImposedReason
                .PackagingMaterial,
            DecisionNotAcceptableActionQuarantineImposedReason.Other => IpaffsDataApi
                .DecisionNotAcceptableActionQuarantineImposedReason
                .Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
