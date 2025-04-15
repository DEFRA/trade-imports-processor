using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ExternalReferenceSystemEnumMapper
{
    public static IpaffsDataApi.ExternalReferenceSystem? Map(ExternalReferenceSystem? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            ExternalReferenceSystem.Ecert => IpaffsDataApi.ExternalReferenceSystem.Ecert,
            ExternalReferenceSystem.Ephyto => IpaffsDataApi.ExternalReferenceSystem.Ephyto,
            ExternalReferenceSystem.Enotification => IpaffsDataApi.ExternalReferenceSystem.Enotification,
            ExternalReferenceSystem.Ncts => IpaffsDataApi.ExternalReferenceSystem.Ncts,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
