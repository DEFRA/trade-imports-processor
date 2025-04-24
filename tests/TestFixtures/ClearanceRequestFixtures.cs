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

    private static string GetRandomDocumentCode()
    {
        var validDocumentCodes = s_validator.CommodityDocumentCheckMap.Select(c => c.DocumentCode).Distinct().ToList();

        return validDocumentCodes[RandomNumberGenerator.GetInt32(0, validDocumentCodes.Count)];
    }

    private static T[] Repeat<T>(Func<T> func)
    {
        var numberRepetitions = RandomNumberGenerator.GetInt32(1, 10);
        return Enumerable.Range(0, numberRepetitions).Select(_ => func()).ToArray();
    }

    private static ClearanceRequestHeader GenerateHeader(int version, string? mrn = null)
    {
        return GetFixture()
            .Build<ClearanceRequestHeader>()
            .With(h => h.EntryReference, mrn ?? GenerateMrn())
            .With(h => h.EntryVersionNumber, version)
            .Create();
    }

    public static IPostprocessComposer<ClearanceRequest> ClearanceRequestFixture(string? mrn = null, int version = 2)
    {
        return GetFixture()
            .Build<ClearanceRequest>()
            .With(c => c.Header, GenerateHeader(version, mrn))
            .With(c => c.ServiceHeader, ServiceHeaderFixture().Create())
            .With(c => c.Items, Repeat(() => ItemFixture().Create()));
    }

    private static IPostprocessComposer<Item> ItemFixture()
    {
        return GetFixture().Build<Item>().With(i => i.Documents, Repeat(() => DocumentFixture().Create()));
    }

    private static IPostprocessComposer<Document> DocumentFixture()
    {
        return GetFixture().Build<Document>().With(d => d.DocumentCode, GetRandomDocumentCode());
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.ClearanceRequest> DataApiClearanceRequestFixture()
    {
        return GetFixture()
            .Build<DataApiCustomsDeclaration.ClearanceRequest>()
            .With(c => c.ExternalVersion, 1)
            .With(c => c.Commodities, Repeat(() => DataApiCommodityFixture().Create()));
    }

    private static IPostprocessComposer<DataApiCustomsDeclaration.Commodity> DataApiCommodityFixture()
    {
        return GetFixture()
            .Build<DataApiCustomsDeclaration.Commodity>()
            .With(c => c.Documents, Repeat(() => DataApiImportDocument().Create()));
    }

    private static IPostprocessComposer<DataApiCustomsDeclaration.ImportDocument> DataApiImportDocument()
    {
        return GetFixture()
            .Build<DataApiCustomsDeclaration.ImportDocument>()
            .With(d => d.DocumentCode, GetRandomDocumentCode());
    }
}
