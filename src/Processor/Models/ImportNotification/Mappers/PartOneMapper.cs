using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartOneMapper
{
    public static IpaffsDataApi.PartOne Map(PartOne? from)
    {
        if (from is null)
            return null!;

        return new IpaffsDataApi.PartOne
        {
            TypeOfImp = from.TypeOfImp,
            PersonResponsible = PartyMapper.Map(from.PersonResponsible),
            CustomsReferenceNumber = from.CustomsReferenceNumber,
            ContainsWoodPackaging = from.ContainsWoodPackaging,
            ConsignmentArrived = from.ConsignmentArrived,
            Consignor = EconomicOperatorMapper.Map(from.Consignor),
            ConsignorTwo = EconomicOperatorMapper.Map(from.ConsignorTwo),
            Packer = EconomicOperatorMapper.Map(from.Packer),
            Consignee = EconomicOperatorMapper.Map(from.Consignee),
            Importer = EconomicOperatorMapper.Map(from.Importer),
            PlaceOfDestination = EconomicOperatorMapper.Map(from.PlaceOfDestination),
            Pod = EconomicOperatorMapper.Map(from.Pod),
            PlaceOfOriginHarvest = EconomicOperatorMapper.Map(from.PlaceOfOriginHarvest),
            AdditionalPermanentAddresses = from
                .AdditionalPermanentAddresses?.Select(EconomicOperatorMapper.Map)
                .ToArray(),
            CphNumber = from.CphNumber,
            ImportingFromCharity = from.ImportingFromCharity,
            IsPlaceOfDestinationThePermanentAddress = from.IsPlaceOfDestinationThePermanentAddress,
            IsCatchCertificateRequired = from.IsCatchCertificateRequired,
            IsGvmsRoute = from.IsGvmsRoute,
            Purpose = PurposeMapper.Map(from.Purpose),
            PointOfEntry = from.PointOfEntry,
            PointOfEntryControlPoint = from.PointOfEntryControlPoint,
            MeansOfTransport = MeansOfTransportMapper.Map(from.MeansOfTransport),
            Transporter = EconomicOperatorMapper.Map(from.Transporter),
            TransporterDetailsRequired = from.TransporterDetailsRequired,
            MeansOfTransportFromEntryPoint = MeansOfTransportMapper.Map(from.MeansOfTransportFromEntryPoint),
            EstimatedJourneyTimeInMinutes = from.EstimatedJourneyTimeInMinutes,
            ResponsibleForTransport = from.ResponsibleForTransport,
            VeterinaryInformation = VeterinaryInformationMapper.Map(from.VeterinaryInformation),
            ImporterLocalReferenceNumber = from.ImporterLocalReferenceNumber,
            Route = RouteMapper.Map(from.Route),
            SealsContainers = from.SealsContainers?.Select(SealContainerMapper.Map).ToArray(),
            StoreTransporterContact = from.StoreTransporterContact,
            SubmittedOn = from.SubmittedOn,
            SubmittedBy = UserInformationMapper.Map(from.SubmittedBy),
            ConsignmentValidations = from.ConsignmentValidations?.Select(ValidationMessageCodeMapper.Map).ToArray(),
            ComplexCommoditySelected = from.ComplexCommoditySelected,
            PortOfEntry = from.PortOfEntry,
            PortOfExit = from.PortOfExit,
            ExitedPortOfOn = from.PortOfExitDate,
            ContactDetails = ContactDetailsMapper.Map(from.ContactDetails),
            NominatedContacts = from.NominatedContacts?.Select(NominatedContactMapper.Map).ToArray(),
            OriginalEstimatedOn = from.OriginalEstimatedOn,
            BillingInformation = BillingInformationMapper.Map(from.BillingInformation),
            IsChargeable = from.IsChargeable,
            WasChargeable = from.WasChargeable,
            CommonUserCharge = CommonUserChargeMapper.Map(from.CommonUserCharge),
            ProvideCtcMrn = from.ProvideCtcMrn,
            ArrivesAt = DateTimeMapper.Map(from.ArrivalDate, from.ArrivalTime),
            DepartedOn = DateTimeMapper.Map(from.DepartureDate, from.DepartureTime),
            Commodities = CommoditiesMapper.Map(from.Commodities),
        };
    }
}
