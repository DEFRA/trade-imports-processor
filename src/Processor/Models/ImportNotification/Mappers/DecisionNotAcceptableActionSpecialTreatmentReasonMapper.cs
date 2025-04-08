#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionNotAcceptableActionSpecialTreatmentReasonEnumMapper
{
    public static IpaffsDataApi.DecisionNotAcceptableActionSpecialTreatmentReason? Map(
        DecisionNotAcceptableActionSpecialTreatmentReason? from
    )
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            DecisionNotAcceptableActionSpecialTreatmentReason.ContaminatedProducts => IpaffsDataApi
                .DecisionNotAcceptableActionSpecialTreatmentReason
                .ContaminatedProducts,
            DecisionNotAcceptableActionSpecialTreatmentReason.InterceptedPart => IpaffsDataApi
                .DecisionNotAcceptableActionSpecialTreatmentReason
                .InterceptedPart,
            DecisionNotAcceptableActionSpecialTreatmentReason.PackagingMaterial => IpaffsDataApi
                .DecisionNotAcceptableActionSpecialTreatmentReason
                .PackagingMaterial,
            DecisionNotAcceptableActionSpecialTreatmentReason.Other => IpaffsDataApi
                .DecisionNotAcceptableActionSpecialTreatmentReason
                .Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
