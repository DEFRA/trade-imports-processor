using System.Security.Cryptography;
using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using static Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations.CommodityValidator;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ClearanceRequestFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    private static AuthorityCodeMap GetRandomDocumentCheckMap()
    {
        var withoutIuuDocuments = AuthorityCodeMappings.Where(d => d.CheckCode != "H224").ToList();

        return withoutIuuDocuments[RandomNumberGenerator.GetInt32(0, withoutIuuDocuments.Count)];
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return new string(
            Enumerable.Repeat(chars, length).Select(s => s[RandomNumberGenerator.GetInt32(0, chars.Length)]).ToArray()
        );
    }

    private static T[] Repeat<T>(Func<int, T> func)
    {
        var numberRepetitions = RandomNumberGenerator.GetInt32(1, 10);
        return Enumerable.Range(1, numberRepetitions + 1).Select(func).ToArray();
    }

    private static ClearanceRequestHeader ClearanceRequestHeaderFixture(int version, string? mrn = null)
    {
        var fixture = GetFixture();

        return fixture
            .Build<ClearanceRequestHeader>()
            .With(h => h.EntryReference, mrn ?? GenerateMrn())
            .With(h => h.EntryVersionNumber, version)
            .With(h => h.PreviousVersionNumber, version - 1)
            .With(h => h.DeclarationUcr, GenerateRandomString(35))
            .With(h => h.DeclarantId, GenerateRandomString(18))
            .With(h => h.DeclarantName, GenerateRandomString(35))
            .With(h => h.DispatchCountryCode, GenerateRandomString(2))
            .With(h => h.DeclarationType, "S")
            .Create();
    }

    public static IPostprocessComposer<ClearanceRequest> ClearanceRequestFixture(string? mrn = null, int version = 2)
    {
        return GetFixture()
            .Build<ClearanceRequest>()
            .With(c => c.Header, ClearanceRequestHeaderFixture(version, mrn))
            .With(c => c.ServiceHeader, ServiceHeaderFixture().Create())
            .With(c => c.Items, Repeat(i => ItemFixture(i).Create()));
    }

    private static IPostprocessComposer<Item> ItemFixture(int index)
    {
        var randomDocumentCheckMap = Repeat(_ => GetRandomDocumentCheckMap());
        var uniqueCheckCodes = randomDocumentCheckMap.DistinctBy(d => d.Name);
        var randomDocumentAndCheckCodes = uniqueCheckCodes
            .OrderBy(_ => Guid.NewGuid())
            .Take(RandomNumberGenerator.GetInt32(1, 3))
            .ToList();

        return GetFixture()
            .Build<Item>()
            .With(i => i.ItemNumber, index)
            .With(i => i.CustomsProcedureCode, RandomNumberGenerator.GetInt32(1, 7).ToString)
            .With(i => i.TaricCommodityCode, RandomNumberGenerator.GetInt32(1000000000, 1999999999).ToString)
            .With(i => i.GoodsDescription, GenerateRandomString(280))
            .With(i => i.ConsigneeId, GenerateRandomString(18))
            .With(i => i.ConsigneeName, GenerateRandomString(35))
            .With(i => i.ItemOriginCountryCode, GenerateRandomString(2))
            .With(
                i => i.Documents,
                randomDocumentAndCheckCodes.Select(d => DocumentFixture(d.DocumentCode).Create()).ToArray()
            )
            .With(i => i.Checks, randomDocumentAndCheckCodes.Select(c => CheckFixture(c.CheckCode).Create()).ToArray());
    }

    private static IPostprocessComposer<Document> DocumentFixture(string documentCode)
    {
        return GetFixture()
            .Build<Document>()
            .With(d => d.DocumentCode, documentCode)
            .With(d => d.DocumentStatus, GenerateRandomString(2))
            .With(d => d.DocumentControl, GenerateRandomString(1));
    }

    private static IPostprocessComposer<Check> CheckFixture(string checkCode)
    {
        return GetFixture()
            .Build<Check>()
            .With(c => c.CheckCode, checkCode)
            .With(c => c.DepartmentCode, GenerateRandomString(8));
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.ClearanceRequest> DataApiClearanceRequestFixture()
    {
        return GetFixture()
            .Build<DataApiCustomsDeclaration.ClearanceRequest>()
            .With(c => c.ExternalVersion, 1)
            .With(c => c.Commodities, Repeat(_ => DataApiCommodityFixture().Create()));
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.Commodity> DataApiCommodityFixture()
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
