using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionNotAcceptableActionEnumMapper
{
    public static IpaffsDataApi.DecisionNotAcceptableAction? Map(DecisionNotAcceptableAction? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            DecisionNotAcceptableAction.Slaughter => IpaffsDataApi.DecisionNotAcceptableAction.Slaughter,
            DecisionNotAcceptableAction.Reexport => IpaffsDataApi.DecisionNotAcceptableAction.Reexport,
            DecisionNotAcceptableAction.Euthanasia => IpaffsDataApi.DecisionNotAcceptableAction.Euthanasia,
            DecisionNotAcceptableAction.Redispatching => IpaffsDataApi.DecisionNotAcceptableAction.Redispatching,
            DecisionNotAcceptableAction.Destruction => IpaffsDataApi.DecisionNotAcceptableAction.Destruction,
            DecisionNotAcceptableAction.Transformation => IpaffsDataApi.DecisionNotAcceptableAction.Transformation,
            DecisionNotAcceptableAction.Other => IpaffsDataApi.DecisionNotAcceptableAction.Other,
            DecisionNotAcceptableAction.EntryRefusal => IpaffsDataApi.DecisionNotAcceptableAction.EntryRefusal,
            DecisionNotAcceptableAction.QuarantineImposed => IpaffsDataApi
                .DecisionNotAcceptableAction
                .QuarantineImposed,
            DecisionNotAcceptableAction.SpecialTreatment => IpaffsDataApi.DecisionNotAcceptableAction.SpecialTreatment,
            DecisionNotAcceptableAction.IndustrialProcessing => IpaffsDataApi
                .DecisionNotAcceptableAction
                .IndustrialProcessing,
            DecisionNotAcceptableAction.ReDispatch => IpaffsDataApi.DecisionNotAcceptableAction.ReDispatch,
            DecisionNotAcceptableAction.UseForOtherPurposes => IpaffsDataApi
                .DecisionNotAcceptableAction
                .UseForOtherPurposes,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
