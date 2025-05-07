using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ValidationMessageCodeMapper
{
    public static IpaffsDataApi.ValidationMessageCode Map(ValidationMessageCode? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ValidationMessageCode { Field = from.Field, Code = from.Code };

        return to;
    }
}
