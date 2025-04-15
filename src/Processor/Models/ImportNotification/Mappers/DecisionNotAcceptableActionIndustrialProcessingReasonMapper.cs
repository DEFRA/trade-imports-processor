using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionNotAcceptableActionIndustrialProcessingReasonEnumMapper
{
    public static IpaffsDataApi.DecisionNotAcceptableActionIndustrialProcessingReason? Map(
        DecisionNotAcceptableActionIndustrialProcessingReason? from
    )
    {
        if (from == null)
            return default!;
        return from switch
        {
            DecisionNotAcceptableActionIndustrialProcessingReason.ContaminatedProducts => IpaffsDataApi
                .DecisionNotAcceptableActionIndustrialProcessingReason
                .ContaminatedProducts,
            DecisionNotAcceptableActionIndustrialProcessingReason.InterceptedPart => IpaffsDataApi
                .DecisionNotAcceptableActionIndustrialProcessingReason
                .InterceptedPart,
            DecisionNotAcceptableActionIndustrialProcessingReason.PackagingMaterial => IpaffsDataApi
                .DecisionNotAcceptableActionIndustrialProcessingReason
                .PackagingMaterial,
            DecisionNotAcceptableActionIndustrialProcessingReason.Other => IpaffsDataApi
                .DecisionNotAcceptableActionIndustrialProcessingReason
                .Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
