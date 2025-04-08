#nullable enable



using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ExternalReferenceMapper
{
    public static IpaffsDataApi.ExternalReference Map(ExternalReference? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.ExternalReference();
        to.System = ExternalReferenceSystemEnumMapper.Map(from?.System);
        to.Reference = from?.Reference;
        to.ExactMatch = from?.ExactMatch;
        to.VerifiedByImporter = from?.VerifiedByImporter;
        to.VerifiedByInspector = from?.VerifiedByInspector;
        return to;
    }
}
