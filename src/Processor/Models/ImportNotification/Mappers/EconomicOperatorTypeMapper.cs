using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class EconomicOperatorTypeEnumMapper
{
    public static IpaffsDataApi.EconomicOperatorType? Map(EconomicOperatorType? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            EconomicOperatorType.Consignee => IpaffsDataApi.EconomicOperatorType.Consignee,
            EconomicOperatorType.Destination => IpaffsDataApi.EconomicOperatorType.Destination,
            EconomicOperatorType.Exporter => IpaffsDataApi.EconomicOperatorType.Exporter,
            EconomicOperatorType.Importer => IpaffsDataApi.EconomicOperatorType.Importer,
            EconomicOperatorType.Charity => IpaffsDataApi.EconomicOperatorType.Charity,
            EconomicOperatorType.CommercialTransporter => IpaffsDataApi.EconomicOperatorType.CommercialTransporter,
            EconomicOperatorType.CommercialTransporterUserAdded => IpaffsDataApi
                .EconomicOperatorType
                .CommercialTransporterUserAdded,
            EconomicOperatorType.PrivateTransporter => IpaffsDataApi.EconomicOperatorType.PrivateTransporter,
            EconomicOperatorType.TemporaryAddress => IpaffsDataApi.EconomicOperatorType.TemporaryAddress,
            EconomicOperatorType.PremisesOfOrigin => IpaffsDataApi.EconomicOperatorType.PremisesOfOrigin,
            EconomicOperatorType.OrganisationBranchAddress => IpaffsDataApi
                .EconomicOperatorType
                .OrganisationBranchAddress,
            EconomicOperatorType.Packer => IpaffsDataApi.EconomicOperatorType.Packer,
            EconomicOperatorType.Pod => IpaffsDataApi.EconomicOperatorType.Pod,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
