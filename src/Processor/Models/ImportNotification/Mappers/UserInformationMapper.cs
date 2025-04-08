#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class UserInformationMapper
{
    public static IpaffsDataApi.UserInformation Map(UserInformation? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.UserInformation();
        to.DisplayName = from?.DisplayName;
        to.UserId = from?.UserId;
        to.IsControlUser = from?.IsControlUser;
        return to;
    }
}
