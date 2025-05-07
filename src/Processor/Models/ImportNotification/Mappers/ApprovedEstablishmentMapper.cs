using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ApprovedEstablishmentMapper
{
    public static IpaffsDataApi.ApprovedEstablishment Map(ApprovedEstablishment? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ApprovedEstablishment
        {
            Id = from.Id,
            Name = from.Name,
            Country = from.Country,
            Types = from.Types,
            ApprovalNumber = from.ApprovalNumber,
            Section = from.Section,
        };

        return to;
    }
}
