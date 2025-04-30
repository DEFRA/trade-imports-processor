using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PurposeMapper
{
    public static IpaffsDataApi.Purpose Map(Purpose? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.Purpose();
        to.ConformsToEU = from.ConformsToEU;
        to.InternalMarketPurpose = from.InternalMarketPurpose;
        to.ThirdCountryTranshipment = from.ThirdCountryTranshipment;
        to.ForNonConforming = from.ForNonConforming;
        to.RegNumber = from.RegNumber;
        to.ShipName = from.ShipName;
        to.ShipPort = from.ShipPort;
        to.ExitBip = from.ExitBip;
        to.ThirdCountry = from.ThirdCountry;
        to.TransitThirdCountries = from.TransitThirdCountries;
        to.ForImportOrAdmission = from.ForImportOrAdmission;
        to.ExitDate = from.ExitDate;
        to.FinalBip = from.FinalBip;
        to.PurposeGroup = from.PurposeGroup;
        to.EstimatedArrivesAtPortOfExit = DateTimeMapper.Map(
            from.EstimatedArrivalDateAtPortOfExit,
            from.EstimatedArrivalTimeAtPortOfExit
        );
        return to;
    }
}
