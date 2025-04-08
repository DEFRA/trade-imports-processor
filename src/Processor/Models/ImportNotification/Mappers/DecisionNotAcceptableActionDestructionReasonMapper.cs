#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionNotAcceptableActionDestructionReasonEnumMapper
{
    public static IpaffsDataApi.DecisionNotAcceptableActionDestructionReason? Map(
        DecisionNotAcceptableActionDestructionReason? from
    )
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            DecisionNotAcceptableActionDestructionReason.ContaminatedProducts => IpaffsDataApi
                .DecisionNotAcceptableActionDestructionReason
                .ContaminatedProducts,
            DecisionNotAcceptableActionDestructionReason.InterceptedPart => IpaffsDataApi
                .DecisionNotAcceptableActionDestructionReason
                .InterceptedPart,
            DecisionNotAcceptableActionDestructionReason.PackagingMaterial => IpaffsDataApi
                .DecisionNotAcceptableActionDestructionReason
                .PackagingMaterial,
            DecisionNotAcceptableActionDestructionReason.Other => IpaffsDataApi
                .DecisionNotAcceptableActionDestructionReason
                .Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
