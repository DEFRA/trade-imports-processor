using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ValidationMessageCodeMapper
{
    public static IpaffsDataApi.ValidationMessageCode Map(ValidationMessageCode? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.ValidationMessageCode();
        to.Field = from.Field;
        to.Code = from.Code;
        return to;
    }
}
