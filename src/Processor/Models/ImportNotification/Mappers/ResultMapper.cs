using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ResultEnumMapper
{
    public static IpaffsDataApi.Result? Map(Result? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            Result.Satisfactory => IpaffsDataApi.Result.Satisfactory,
            Result.SatisfactoryFollowingOfficialIntervention => IpaffsDataApi
                .Result
                .SatisfactoryFollowingOfficialIntervention,
            Result.NotSatisfactory => IpaffsDataApi.Result.NotSatisfactory,
            Result.NotDone => IpaffsDataApi.Result.NotDone,
            Result.Derogation => IpaffsDataApi.Result.Derogation,
            Result.NotSet => IpaffsDataApi.Result.NotSet,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
