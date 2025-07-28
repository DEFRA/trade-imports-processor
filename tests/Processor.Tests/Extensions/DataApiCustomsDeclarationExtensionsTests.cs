using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Extensions;

namespace Defra.TradeImportsProcessor.Processor.Tests.Extensions;

public class DataApiCustomsDeclarationExtensionsTests
{
    [Fact]
    public void State_ClearanceRequest_Empty()
    {
        var subject = new CustomsDeclaration { ClearanceRequest = new ClearanceRequest() };

        var result = subject.State();

        result.Should().Be("ClearanceRequest[MessageSentAt=0001-01-01T00:00:00.0000000, ExternalVersion=]");
    }

    [Fact]
    public void State_ClearanceRequest_WithData()
    {
        var subject = new CustomsDeclaration
        {
            ClearanceRequest = new ClearanceRequest
            {
                MessageSentAt = new DateTime(2025, 7, 28, 14, 23, 01, DateTimeKind.Utc),
                ExternalVersion = 2,
            },
        };

        var result = subject.State();

        result.Should().Be("ClearanceRequest[MessageSentAt=2025-07-28T14:23:01.0000000Z, ExternalVersion=2]");
    }

    [Fact]
    public void State_Decision_Empty()
    {
        var subject = new CustomsDeclaration { ClearanceDecision = new ClearanceDecision { Items = [] } };

        var result = subject.State();

        result.Should().Be("ClearanceDecision[DecisionNumber=]");
    }

    [Fact]
    public void State_Decision_WithData()
    {
        var subject = new CustomsDeclaration
        {
            ClearanceDecision = new ClearanceDecision { Items = [], DecisionNumber = 2 },
        };

        var result = subject.State();

        result.Should().Be("ClearanceDecision[DecisionNumber=2]");
    }

    [Fact]
    public void State_Finalisation_Empty()
    {
        var subject = new CustomsDeclaration
        {
            Finalisation = new Finalisation
            {
                ExternalVersion = 0,
                FinalState = "0",
                IsManualRelease = false,
            },
        };

        var result = subject.State();

        result.Should().Be("Finalisation[MessageSentAt=0001-01-01T00:00:00.0000000, ExternalVersion=0]");
    }

    [Fact]
    public void State_Finalisation_WithData()
    {
        var subject = new CustomsDeclaration
        {
            Finalisation = new Finalisation
            {
                ExternalVersion = 2,
                FinalState = "0",
                IsManualRelease = false,
                MessageSentAt = new DateTime(2025, 7, 28, 14, 23, 01, DateTimeKind.Utc),
            },
        };

        var result = subject.State();

        result.Should().Be("Finalisation[MessageSentAt=2025-07-28T14:23:01.0000000Z, ExternalVersion=2]");
    }

    [Fact]
    public void State_All()
    {
        var subject = new CustomsDeclaration
        {
            ClearanceRequest = new ClearanceRequest
            {
                MessageSentAt = new DateTime(2025, 7, 28, 14, 23, 01, DateTimeKind.Utc),
                ExternalVersion = 2,
            },
            ClearanceDecision = new ClearanceDecision { Items = [], DecisionNumber = 2 },
            Finalisation = new Finalisation
            {
                ExternalVersion = 2,
                FinalState = "0",
                IsManualRelease = false,
                MessageSentAt = new DateTime(2025, 7, 28, 14, 23, 01, DateTimeKind.Utc),
            },
        };

        var result = subject.State();

        result
            .Should()
            .Be(
                "ClearanceRequest[MessageSentAt=2025-07-28T14:23:01.0000000Z, ExternalVersion=2], ClearanceDecision[DecisionNumber=2], Finalisation[MessageSentAt=2025-07-28T14:23:01.0000000Z, ExternalVersion=2]"
            );
    }

    [Fact]
    public void State_All_Null()
    {
        var subject = new CustomsDeclaration();

        var result = subject.State();

        result.Should().BeNull();
    }
}
