#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ApprovedEstablishmentMapper
{
    public static IpaffsDataApi.ApprovedEstablishment Map(ApprovedEstablishment? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.ApprovedEstablishment();
        to.Id = from?.Id;
        to.Name = from?.Name;
        to.Country = from?.Country;
        to.Types = from?.Types;
        to.ApprovalNumber = from?.ApprovalNumber;
        to.Section = from?.Section;
        return to;
    }
}
