using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class UserInformationMapper
{
    public static IpaffsDataApi.UserInformation Map(UserInformation? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.UserInformation
        {
            DisplayName = from.DisplayName,
            UserId = from.UserId,
            IsControlUser = from.IsControlUser,
        };

        return to;
    }
}
