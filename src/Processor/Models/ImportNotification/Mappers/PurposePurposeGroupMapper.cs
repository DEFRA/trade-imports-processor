#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PurposePurposeGroupEnumMapper
{
    public static IpaffsDataApi.PurposePurposeGroup? Map(PurposePurposeGroup? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            PurposePurposeGroup.ForImport => IpaffsDataApi.PurposePurposeGroup.ForImport,
            PurposePurposeGroup.ForNONConformingConsignments => IpaffsDataApi
                .PurposePurposeGroup
                .ForNONConformingConsignments,
            PurposePurposeGroup.ForTranshipmentTo => IpaffsDataApi.PurposePurposeGroup.ForTranshipmentTo,
            PurposePurposeGroup.ForTransitTo3rdCountry => IpaffsDataApi.PurposePurposeGroup.ForTransitTo3rdCountry,
            PurposePurposeGroup.ForReImport => IpaffsDataApi.PurposePurposeGroup.ForReImport,
            PurposePurposeGroup.ForPrivateImport => IpaffsDataApi.PurposePurposeGroup.ForPrivateImport,
            PurposePurposeGroup.ForTransferTo => IpaffsDataApi.PurposePurposeGroup.ForTransferTo,
            PurposePurposeGroup.ForImportReConformityCheck => IpaffsDataApi
                .PurposePurposeGroup
                .ForImportReConformityCheck,
            PurposePurposeGroup.ForImportNonInternalMarket => IpaffsDataApi
                .PurposePurposeGroup
                .ForImportNonInternalMarket,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
