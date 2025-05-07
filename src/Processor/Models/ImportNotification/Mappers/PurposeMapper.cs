using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PurposeMapper
{
    public static IpaffsDataApi.Purpose Map(Purpose? from)
    {
        if (from is null)
            return null!;

        return new IpaffsDataApi.Purpose
        {
            ConformsToEU = from.ConformsToEU,
            InternalMarketPurpose = from.InternalMarketPurpose,
            ThirdCountryTranshipment = from.ThirdCountryTranshipment,
            ForNonConforming = from.ForNonConforming,
            RegNumber = from.RegNumber,
            ShipName = from.ShipName,
            ShipPort = from.ShipPort,
            ExitBip = from.ExitBip,
            ThirdCountry = from.ThirdCountry,
            TransitThirdCountries = from.TransitThirdCountries,
            ForImportOrAdmission = from.ForImportOrAdmission,
            ExitDate = from.ExitDate,
            FinalBip = from.FinalBip,
            PointOfExit = from.PointOfExit,
            PurposeGroup = from.PurposeGroup,
            EstimatedArrivesAtPortOfExit = DateTimeMapper.Map(
                from.EstimatedArrivalDateAtPortOfExit,
                from.EstimatedArrivalTimeAtPortOfExit
            ),
        };
    }
}
