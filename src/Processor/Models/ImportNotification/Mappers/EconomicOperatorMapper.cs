#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class EconomicOperatorMapper
{
    public static IpaffsDataApi.EconomicOperator Map(EconomicOperator? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.EconomicOperator();
        to.Id = from?.Id;
        to.Type = EconomicOperatorTypeEnumMapper.Map(from?.Type);
        to.Status = EconomicOperatorStatusEnumMapper.Map(from?.Status);
        to.CompanyName = from?.CompanyName;
        to.IndividualName = from?.IndividualName;
        to.Address = AddressMapper.Map(from?.Address);
        to.ApprovalNumber = from?.ApprovalNumber;
        to.OtherIdentifier = from?.OtherIdentifier;
        to.TracesId = from?.TracesId;
        return to;
    }
}
