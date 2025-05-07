namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

public static class ImportNotificationWithTransformMapper
{
    public static IpaffsDataApi.ImportPreNotification MapWithTransform(this ImportNotification? from)
    {
        if (from is null)
        {
            return null!;
        }

        var notification = ImportNotificationMapper.Map(from);
        Map(from, notification);
        return notification;
    }

    private static void Map(ImportNotification from, IpaffsDataApi.ImportPreNotification to)
    {
        var commodities = from.PartOne!.Commodities;

        if (commodities?.CommodityComplements?.Length == 1)
        {
            commodities.CommodityComplements[0].AdditionalData = commodities.ComplementParameterSets![0].KeyDataPairs;

            if (from.RiskAssessment != null)
            {
                commodities.CommodityComplements[0].RiskAssesment = from.RiskAssessment.CommodityResults![0];
            }
        }
        else
        {
            var complementParameters = new Dictionary<int, ComplementParameterSet>();
            var complementRiskAssesments = new Dictionary<string, CommodityRiskResult>();
            var commodityChecks = new Dictionary<string, InspectionCheck[]>();

            if (commodities?.ComplementParameterSets != null)
            {
                foreach (var commoditiesCommodityComplement in commodities.ComplementParameterSets)
                {
                    complementParameters[commoditiesCommodityComplement.ComplementId!.Value] =
                        commoditiesCommodityComplement;
                }
            }

            if (from.RiskAssessment?.CommodityResults != null)
            {
                foreach (var commoditiesRa in from.RiskAssessment.CommodityResults)
                {
                    complementRiskAssesments[commoditiesRa.UniqueId!] = commoditiesRa;
                }
            }

            if (from.PartTwo?.CommodityChecks != null)
            {
                foreach (var commodityCheck in from.PartTwo.CommodityChecks!)
                {
                    commodityChecks[commodityCheck.UniqueComplementId!] = commodityCheck.Checks!;
                }
            }

            if (commodities?.CommodityComplements is not null)
            {
                foreach (var commodity in commodities.CommodityComplements)
                {
                    var parameters = complementParameters[commodity.ComplementId!.Value];
                    commodity.AdditionalData = parameters.KeyDataPairs;

                    if (
                        complementRiskAssesments.Any()
                        && parameters.UniqueComplementId is not null
                        && complementRiskAssesments.ContainsKey(parameters.UniqueComplementId)
                    )
                    {
                        commodity.RiskAssesment = complementRiskAssesments[parameters.UniqueComplementId!];
                    }

                    if (
                        commodityChecks.Any()
                        && parameters.UniqueComplementId is not null
                        && commodityChecks.ContainsKey(parameters.UniqueComplementId)
                    )
                    {
                        commodity.Checks = commodityChecks[parameters.UniqueComplementId!];
                    }
                }
            }
        }

        if (commodities != null)
        {
            to.CommoditiesSummary = CommoditiesMapper.Map(commodities);
            to.Commodities = commodities.CommodityComplements?.Select(x => CommodityComplementMapper.Map(x)).ToArray()!;
        }
    }
}
