using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionDecisionEnumMapper
{
    public static IpaffsDataApi.ConsignmentDecision? Map(DecisionDecision? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            DecisionDecision.NonAcceptable => IpaffsDataApi.ConsignmentDecision.NonAcceptable,
            DecisionDecision.AcceptableForInternalMarket => IpaffsDataApi
                .ConsignmentDecision
                .AcceptableForInternalMarket,
            DecisionDecision.AcceptableIfChanneled => IpaffsDataApi.ConsignmentDecision.AcceptableIfChanneled,
            DecisionDecision.AcceptableForTranshipment => IpaffsDataApi.ConsignmentDecision.AcceptableForTranshipment,
            DecisionDecision.AcceptableForTransit => IpaffsDataApi.ConsignmentDecision.AcceptableForTransit,
            DecisionDecision.AcceptableForTemporaryImport => IpaffsDataApi
                .ConsignmentDecision
                .AcceptableForTemporaryImport,
            DecisionDecision.AcceptableForSpecificWarehouse => IpaffsDataApi
                .ConsignmentDecision
                .AcceptableForSpecificWarehouse,
            DecisionDecision.AcceptableForPrivateImport => IpaffsDataApi.ConsignmentDecision.AcceptableForPrivateImport,
            DecisionDecision.AcceptableForTransfer => IpaffsDataApi.ConsignmentDecision.AcceptableForTransfer,
            DecisionDecision.HorseReEntry => IpaffsDataApi.ConsignmentDecision.HorseReEntry,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
