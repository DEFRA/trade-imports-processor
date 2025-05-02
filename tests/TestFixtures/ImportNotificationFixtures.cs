using System.Security.Cryptography;
using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using ImportNotificationStatus = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotificationStatus;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ImportNotificationFixtures
{
    private static readonly List<string> s_chedTypes = ["A", "D", "P", "PP"];

    private static Fixture GetFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
        return fixture;
    }

    private static string GenerateReferenceNumber()
    {
        var chedType = s_chedTypes[RandomNumberGenerator.GetInt32(0, s_chedTypes.Count)];
        var currentYear = DateTime.Now.Year;
        var number = RandomNumberGenerator.GetInt32(1000000, 10000000);

        return $"CHED{chedType}.GB.{currentYear}.{number}";
    }

    private static IPostprocessComposer<PartOne> PartOneFixture()
    {
        var commodityComplements = new List<CommodityComplement>();
        var complementParameterSet = new List<ComplementParameterSet>();

        for (var id = 1; id < 4; id++)
        {
            commodityComplements.Add(
                GetFixture().Build<CommodityComplement>().With(comp => comp.ComplementId, id).Create()
            );
            complementParameterSet.Add(
                GetFixture().Build<ComplementParameterSet>().With(param => param.ComplementId, id).Create()
            );
        }

        var commodities = GetFixture()
            .Build<Commodities>()
            .With(c => c.CommodityComplements, commodityComplements.ToArray())
            .With(c => c.ComplementParameterSets, complementParameterSet.ToArray());

        return GetFixture().Build<PartOne>().With(p => p.Commodities, commodities.Create());
    }

    public static IPostprocessComposer<ImportNotification> ImportNotificationFixture()
    {
        return GetFixture()
            .Build<ImportNotification>()
            .With(i => i.ReferenceNumber, GenerateReferenceNumber())
            .With(i => i.LastUpdated, DateTime.UtcNow)
            .With(i => i.Status, ImportNotificationStatus.InProgress)
            .With(i => i.PartOne, PartOneFixture().Create());
    }
}
