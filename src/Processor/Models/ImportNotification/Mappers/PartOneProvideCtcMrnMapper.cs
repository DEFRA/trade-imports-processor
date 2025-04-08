#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartOneProvideCtcMrnEnumMapper
{
    public static IpaffsDataApi.PartOneProvideCtcMrn? Map(PartOneProvideCtcMrn? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            PartOneProvideCtcMrn.Yes => IpaffsDataApi.PartOneProvideCtcMrn.Yes,
            PartOneProvideCtcMrn.YesAddLater => IpaffsDataApi.PartOneProvideCtcMrn.YesAddLater,
            PartOneProvideCtcMrn.No => IpaffsDataApi.PartOneProvideCtcMrn.No,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
