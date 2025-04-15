using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionNotAcceptableActionReDispatchReasonEnumMapper
{
    public static IpaffsDataApi.DecisionNotAcceptableActionReDispatchReason? Map(
        DecisionNotAcceptableActionReDispatchReason? from
    )
    {
        if (from == null)
            return default!;
        return from switch
        {
            DecisionNotAcceptableActionReDispatchReason.ContaminatedProducts => IpaffsDataApi
                .DecisionNotAcceptableActionReDispatchReason
                .ContaminatedProducts,
            DecisionNotAcceptableActionReDispatchReason.InterceptedPart => IpaffsDataApi
                .DecisionNotAcceptableActionReDispatchReason
                .InterceptedPart,
            DecisionNotAcceptableActionReDispatchReason.PackagingMaterial => IpaffsDataApi
                .DecisionNotAcceptableActionReDispatchReason
                .PackagingMaterial,
            DecisionNotAcceptableActionReDispatchReason.MeansOfTransport => IpaffsDataApi
                .DecisionNotAcceptableActionReDispatchReason
                .MeansOfTransport,
            DecisionNotAcceptableActionReDispatchReason.Other => IpaffsDataApi
                .DecisionNotAcceptableActionReDispatchReason
                .Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
