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
        to.InternalMarketPurpose = PurposeInternalMarketPurposeEnumMapper.Map(from.InternalMarketPurpose);
        to.ThirdCountryTranshipment = from.ThirdCountryTranshipment;
        to.ForNonConforming = PurposeForNonConformingEnumMapper.Map(from.ForNonConforming);
        to.RegNumber = from.RegNumber;
        to.ShipName = from.ShipName;
        to.ShipPort = from.ShipPort;
        to.ExitBip = from.ExitBip;
        to.ThirdCountry = from.ThirdCountry;
        to.TransitThirdCountries = from.TransitThirdCountries;
        to.ForImportOrAdmission = PurposeForImportOrAdmissionEnumMapper.Map(from.ForImportOrAdmission);
        to.ExitDate = from.ExitDate;
        to.FinalBip = from.FinalBip;
        to.PurposeGroup = PurposePurposeGroupEnumMapper.Map(from.PurposeGroup);
        to.EstimatedArrivesAtPortOfExit = DateTimeMapper.Map(
            from.EstimatedArrivalDateAtPortOfExit,
            from.EstimatedArrivalTimeAtPortOfExit
        );
        return to;
    }
}
