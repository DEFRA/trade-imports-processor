using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using static Defra.TradeImportsProcessor.TestFixtures.GmrFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class MatchedGmrTests
{
    [Fact]
    public void MatchedGmr_DeserializesCorrectly_WithMrn()
    {
        const string json = """
            {
                "mrn": "23GB123456789012345",
                "gmr": {
                    "gmrId": "GMR-001",
                    "haulierEORI": "GB123456789000",
                    "state": "Arrived",
                    "inspectionRequired": false,
                    "updatedDateTime": "2024-01-15T10:30:00Z",
                    "direction": "GB_TO_NI",
                    "haulierType": "HAULIER"
                }
            }
            """;

        var matchedGmr = JsonSerializer.Deserialize<MatchedGmr>(json);

        matchedGmr.Should().NotBeNull();
        matchedGmr.Mrn.Should().Be("23GB123456789012345");
        matchedGmr.Gmr.Should().NotBeNull();
        matchedGmr.Gmr.GmrId.Should().Be("GMR-001");
    }

    [Fact]
    public void MatchedGmr_DeserializesCorrectly_WithoutMrn()
    {
        const string json = """
            {
                "gmr": {
                    "gmrId": "GMR-002",
                    "haulierEORI": "GB987654321000",
                    "state": "Pending",
                    "inspectionRequired": true,
                    "updatedDateTime": "2024-01-16T14:45:00Z",
                    "direction": "NI_TO_GB",
                    "haulierType": "CARRIER"
                }
            }
            """;

        var matchedGmr = JsonSerializer.Deserialize<MatchedGmr>(json);

        matchedGmr.Should().NotBeNull();
        matchedGmr.Mrn.Should().BeNull();
        matchedGmr.Gmr.Should().NotBeNull();
        matchedGmr.Gmr.GmrId.Should().Be("GMR-002");
    }

    [Fact]
    public void GetIdentifier_ReturnsMrnAndGmrId_WhenMrnPresent()
    {
        var gmr = GmrFixture().With(x => x.GmrId, "GMR-123").Create();
        var matchedGmr = new MatchedGmr { Mrn = "23GB999888777666555", Gmr = gmr };

        var identifier = matchedGmr.GetIdentifier;

        identifier.Should().Be("23GB999888777666555-GMR-123");
    }

    [Fact]
    public void GetIdentifier_ReturnsUnknownAndGmrId_WhenMrnIsNull()
    {
        var gmr = GmrFixture().With(x => x.GmrId, "GMR-456").Create();
        var matchedGmr = new MatchedGmr { Mrn = null, Gmr = gmr };

        var identifier = matchedGmr.GetIdentifier;

        identifier.Should().Be("Unknown-GMR-456");
    }
}
