#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionNotAcceptableActionUseForOtherPurposesReasonEnumMapper
{
    public static IpaffsDataApi.DecisionNotAcceptableActionUseForOtherPurposesReason? Map(
        DecisionNotAcceptableActionUseForOtherPurposesReason? from
    )
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            DecisionNotAcceptableActionUseForOtherPurposesReason.ContaminatedProducts => IpaffsDataApi
                .DecisionNotAcceptableActionUseForOtherPurposesReason
                .ContaminatedProducts,
            DecisionNotAcceptableActionUseForOtherPurposesReason.InterceptedPart => IpaffsDataApi
                .DecisionNotAcceptableActionUseForOtherPurposesReason
                .InterceptedPart,
            DecisionNotAcceptableActionUseForOtherPurposesReason.PackagingMaterial => IpaffsDataApi
                .DecisionNotAcceptableActionUseForOtherPurposesReason
                .PackagingMaterial,
            DecisionNotAcceptableActionUseForOtherPurposesReason.MeansOfTransport => IpaffsDataApi
                .DecisionNotAcceptableActionUseForOtherPurposesReason
                .MeansOfTransport,
            DecisionNotAcceptableActionUseForOtherPurposesReason.Other => IpaffsDataApi
                .DecisionNotAcceptableActionUseForOtherPurposesReason
                .Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
