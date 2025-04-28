using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartOneMapper
{
    public static IpaffsDataApi.PartOne Map(PartOne? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.PartOne();
        to.TypeOfImp = PartOneTypeOfImpEnumMapper.Map(from.TypeOfImp);
        to.PersonResponsible = PartyMapper.Map(from.PersonResponsible);
        to.CustomsReferenceNumber = from.CustomsReferenceNumber;
        to.ContainsWoodPackaging = from.ContainsWoodPackaging;
        to.ConsignmentArrived = from.ConsignmentArrived;
        to.Consignor = EconomicOperatorMapper.Map(from.Consignor);
        to.ConsignorTwo = EconomicOperatorMapper.Map(from.ConsignorTwo);
        to.Packer = EconomicOperatorMapper.Map(from.Packer);
        to.Consignee = EconomicOperatorMapper.Map(from.Consignee);
        to.Importer = EconomicOperatorMapper.Map(from.Importer);
        to.PlaceOfDestination = EconomicOperatorMapper.Map(from.PlaceOfDestination);
        to.Pod = EconomicOperatorMapper.Map(from.Pod);
        to.PlaceOfOriginHarvest = EconomicOperatorMapper.Map(from.PlaceOfOriginHarvest);
        to.AdditionalPermanentAddresses = from
            .AdditionalPermanentAddresses?.Select(x => EconomicOperatorMapper.Map(x))
            .ToArray();
        to.CphNumber = from.CphNumber;
        to.ImportingFromCharity = from.ImportingFromCharity;
        to.IsPlaceOfDestinationThePermanentAddress = from.IsPlaceOfDestinationThePermanentAddress;
        to.IsCatchCertificateRequired = from.IsCatchCertificateRequired;
        to.IsGvmsRoute = from.IsGvmsRoute;
        to.Purpose = PurposeMapper.Map(from.Purpose);
        to.PointOfEntry = from.PointOfEntry;
        to.PointOfEntryControlPoint = from.PointOfEntryControlPoint;
        to.MeansOfTransport = MeansOfTransportMapper.Map(from.MeansOfTransport);
        to.Transporter = EconomicOperatorMapper.Map(from.Transporter);
        to.TransporterDetailsRequired = from.TransporterDetailsRequired;
        to.MeansOfTransportFromEntryPoint = MeansOfTransportMapper.Map(from.MeansOfTransportFromEntryPoint);
        to.EstimatedJourneyTimeInMinutes = from.EstimatedJourneyTimeInMinutes;
        to.ResponsibleForTransport = from.ResponsibleForTransport;
        to.VeterinaryInformation = VeterinaryInformationMapper.Map(from.VeterinaryInformation);
        to.ImporterLocalReferenceNumber = from.ImporterLocalReferenceNumber;
        to.Route = RouteMapper.Map(from.Route);
        to.SealsContainers = from.SealsContainers?.Select(x => SealContainerMapper.Map(x)).ToArray();
        to.SubmittedOn = from.SubmittedOn;
        to.SubmittedBy = UserInformationMapper.Map(from.SubmittedBy);
        to.ConsignmentValidations = from
            .ConsignmentValidations?.Select(x => ValidationMessageCodeMapper.Map(x))
            .ToArray();
        to.ComplexCommoditySelected = from.ComplexCommoditySelected;
        to.PortOfEntry = from.PortOfEntry;
        to.PortOfExit = from.PortOfExit;
        to.ExitedPortOfOn = from.PortOfExitDate;
        to.ContactDetails = ContactDetailsMapper.Map(from.ContactDetails);
        to.NominatedContacts = from.NominatedContacts?.Select(x => NominatedContactMapper.Map(x)).ToArray();
        to.OriginalEstimatedOn = from.OriginalEstimatedOn;
        to.BillingInformation = BillingInformationMapper.Map(from.BillingInformation);
        to.IsChargeable = from.IsChargeable;
        to.WasChargeable = from.WasChargeable;
        to.CommonUserCharge = CommonUserChargeMapper.Map(from.CommonUserCharge);
        to.ProvideCtcMrn = PartOneProvideCtcMrnEnumMapper.Map(from.ProvideCtcMrn);
        to.ArrivesAt = DateTimeMapper.Map(from.ArrivalDate, from.ArrivalTime);
        to.DepartedOn = DateTimeMapper.Map(from.DepartureDate, from.DepartureTime);
        return to;
    }
}
