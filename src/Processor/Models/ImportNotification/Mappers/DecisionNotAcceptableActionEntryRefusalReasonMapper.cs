using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionNotAcceptableActionEntryRefusalReasonEnumMapper
{
    public static IpaffsDataApi.DecisionNotAcceptableActionEntryRefusalReason? Map(
        DecisionNotAcceptableActionEntryRefusalReason? from
    )
    {
        if (from == null)
            return default!;
        return from switch
        {
            DecisionNotAcceptableActionEntryRefusalReason.ContaminatedProducts => IpaffsDataApi
                .DecisionNotAcceptableActionEntryRefusalReason
                .ContaminatedProducts,
            DecisionNotAcceptableActionEntryRefusalReason.InterceptedPart => IpaffsDataApi
                .DecisionNotAcceptableActionEntryRefusalReason
                .InterceptedPart,
            DecisionNotAcceptableActionEntryRefusalReason.PackagingMaterial => IpaffsDataApi
                .DecisionNotAcceptableActionEntryRefusalReason
                .PackagingMaterial,
            DecisionNotAcceptableActionEntryRefusalReason.MeansOfTransport => IpaffsDataApi
                .DecisionNotAcceptableActionEntryRefusalReason
                .MeansOfTransport,
            DecisionNotAcceptableActionEntryRefusalReason.Other => IpaffsDataApi
                .DecisionNotAcceptableActionEntryRefusalReason
                .Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
