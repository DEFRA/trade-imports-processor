using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class EconomicOperatorMapper
{
    public static IpaffsDataApi.EconomicOperator Map(EconomicOperator? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.EconomicOperator
        {
            Id = from.Id,
            Type = from.Type,
            Status = from.Status,
            CompanyName = from.CompanyName,
            IndividualName = from.IndividualName,
            Address = AddressMapper.Map(from.Address),
            ApprovalNumber = from.ApprovalNumber,
            OtherIdentifier = from.OtherIdentifier,
            TracesId = from.TracesId,
        };

        return to;
    }
}
