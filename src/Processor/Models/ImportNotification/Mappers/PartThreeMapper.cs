using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartThreeMapper
{
    public static IpaffsDataApi.PartThree Map(PartThree? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.PartThree
        {
            ControlStatus = from.ControlStatus,
            Control = ControlMapper.Map(from.Control),
            ConsignmentValidations = from
                .ConsignmentValidations?.Select(x => ValidationMessageCodeMapper.Map(x))
                .ToArray(),
            SealCheckRequired = from.SealCheckRequired,
            SealCheck = SealCheckMapper.Map(from.SealCheck),
            SealCheckOverride = InspectionOverrideMapper.Map(from.SealCheckOverride),
        };

        return to;
    }
}
