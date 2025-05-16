using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ComplementParameterSetMapper
{
    public static IpaffsDataApi.ComplementParameterSet Map(ComplementParameterSet? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ComplementParameterSet
        {
            UniqueComplementId = from.UniqueComplementId,
            ComplementId = from.ComplementId,
            SpeciesId = from.SpeciesId,
            KeyDataPairs = from.KeyDataPairs,
            CatchCertificates = from.CatchCertificates?.Select(CatchCertificatesMapper.Map).ToArray(),
            Identifiers = from.Identifiers?.Select(IdentifiersMapper.Map).ToArray(),
        };

        return to;
    }
}
