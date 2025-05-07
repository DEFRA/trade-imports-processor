using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ExternalReferenceMapper
{
    public static IpaffsDataApi.ExternalReference Map(ExternalReference? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ExternalReference
        {
            System = from.System,
            Reference = from.Reference,
            ExactMatch = from.ExactMatch,
            VerifiedByImporter = from.VerifiedByImporter,
            VerifiedByInspector = from.VerifiedByInspector,
        };

        return to;
    }
}
