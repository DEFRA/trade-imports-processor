using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionDefinitiveImportPurposeEnumMapper
{
    public static IpaffsDataApi.DecisionDefinitiveImportPurpose? Map(DecisionDefinitiveImportPurpose? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            DecisionDefinitiveImportPurpose.Slaughter => IpaffsDataApi.DecisionDefinitiveImportPurpose.Slaughter,
            DecisionDefinitiveImportPurpose.Approvedbodies => IpaffsDataApi
                .DecisionDefinitiveImportPurpose
                .Approvedbodies,
            DecisionDefinitiveImportPurpose.Quarantine => IpaffsDataApi.DecisionDefinitiveImportPurpose.Quarantine,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
