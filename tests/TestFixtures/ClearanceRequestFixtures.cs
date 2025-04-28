using System.Security.Cryptography;
using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ClearanceRequestFixtures
{
    private static readonly ClearanceRequestValidator s_validator = new();

    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    private static CommodityDocumentCheckMap GetRandomDocumentCheckMap()
    {
        var withoutIuuDocuments = s_validator.CommodityDocumentCheckMap.Where(d => d.CheckCode != "H224").ToList();

        return withoutIuuDocuments[RandomNumberGenerator.GetInt32(0, withoutIuuDocuments.Count)];
    }

    private static T[] Repeat<T>(Func<int, T> func)
    {
        var numberRepetitions = RandomNumberGenerator.GetInt32(1, 10);
        return Enumerable.Range(1, numberRepetitions + 1).Select(func).ToArray();
    }

    private static ClearanceRequestHeader GenerateHeader(int version, string? mrn = null)
    {
        return GetFixture()
            .Build<ClearanceRequestHeader>()
            .With(h => h.EntryReference, mrn ?? GenerateMrn())
            .With(h => h.EntryVersionNumber, version)
            .With(h => h.PreviousVersionNumber, version - 1)
            .Create();
    }

    public static IPostprocessComposer<ClearanceRequest> ClearanceRequestFixture(string? mrn = null, int version = 2)
    {
        return GetFixture()
            .Build<ClearanceRequest>()
            .With(c => c.Header, GenerateHeader(version, mrn))
            .With(c => c.ServiceHeader, ServiceHeaderFixture().Create())
            .With(c => c.Items, Repeat(i => ItemFixture(i).Create()));
    }

    private static IPostprocessComposer<Item> ItemFixture(int index)
    {
        var randomDocumentCheckMap = Repeat(_ => GetRandomDocumentCheckMap());
        var checkCodes = randomDocumentCheckMap.Select(d => d.CheckCode).ToArray();
        var documentCodes = randomDocumentCheckMap.Select(d => d.DocumentCode).ToArray();

        return GetFixture()
            .Build<Item>()
            .With(i => i.ItemNumber, index)
            .With(i => i.Documents, documentCodes.Select(d => DocumentFixture(d).Create()).ToArray())
            .With(i => i.Checks, checkCodes.Select(c => CheckFixture(c).Create()).ToArray());
    }

    private static IPostprocessComposer<Document> DocumentFixture(string documentCode)
    {
        return GetFixture().Build<Document>().With(d => d.DocumentCode, documentCode);
    }

    private static IPostprocessComposer<Check> CheckFixture(string checkCode)
    {
        return GetFixture().Build<Check>().With(c => c.CheckCode, checkCode);
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.ClearanceRequest> DataApiClearanceRequestFixture()
    {
        return GetFixture()
            .Build<DataApiCustomsDeclaration.ClearanceRequest>()
            .With(c => c.ExternalVersion, 1)
            .With(c => c.Commodities, Repeat(_ => DataApiCommodityFixture().Create()));
    }

    private static IPostprocessComposer<DataApiCustomsDeclaration.Commodity> DataApiCommodityFixture()
    {
        return GetFixture()
            .Build<DataApiCustomsDeclaration.Commodity>()
            .With(c => c.Documents, Repeat(_ => DataApiImportDocument().Create()));
    }

    private static IPostprocessComposer<DataApiCustomsDeclaration.ImportDocument> DataApiImportDocument()
    {
        return GetFixture().Build<DataApiCustomsDeclaration.ImportDocument>();
    }
}
