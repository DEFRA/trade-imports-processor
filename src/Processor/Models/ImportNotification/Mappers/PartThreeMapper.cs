using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartThreeMapper
{
    public static IpaffsDataApi.PartThree Map(PartThree? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.PartThree();
        to.ControlStatus = from.ControlStatus;
        to.Control = ControlMapper.Map(from.Control);
        to.ConsignmentValidations = from
            .ConsignmentValidations?.Select(x => ValidationMessageCodeMapper.Map(x))
            .ToArray();
        to.SealCheckRequired = from.SealCheckRequired;
        to.SealCheck = SealCheckMapper.Map(from.SealCheck);
        to.SealCheckOverride = InspectionOverrideMapper.Map(from.SealCheckOverride);
        return to;
    }
}
