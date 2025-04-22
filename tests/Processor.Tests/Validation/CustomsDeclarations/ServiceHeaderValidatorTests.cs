using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class ServiceHeaderValidatorTests
{
    private readonly ServiceHeaderValidator _validator = new();

    [Theory]
    [InlineData("NOT_CDS", true)]
    [InlineData("CDS", false)]
    private void Validate_SourceSystem_ALVSVAL101_Validation(string sourceSystemValue, bool shouldError)
    {
        var serviceHeader = ServiceHeaderFixture().With(sh => sh.SourceSystem, sourceSystemValue).Create();

        var result = _validator.Validate(serviceHeader);
        var hasError = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL101") != null;

        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData("NOT_ALVS", true)]
    [InlineData("ALVS", false)]
    private void Validate_DestinationSystem_ALVSVAL102_Validation(string destinationSystemValue, bool shouldError)
    {
        var serviceHeader = ServiceHeaderFixture().With(sh => sh.DestinationSystem, destinationSystemValue).Create();

        var result = _validator.Validate(serviceHeader);
        var hasError = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL102") != null;

        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("A", false)]
    [InlineData("ABCDABCDABCDABCDABC", false)]
    [InlineData("ABCDABCDABCDABCDABCDABCD", true)]
    private void Validate_CorrelationId_Validation(string correlationId, bool shouldError)
    {
        var serviceHeader = ServiceHeaderFixture().With(sh => sh.CorrelationId, correlationId).Create();

        var result = _validator.Validate(serviceHeader);
        var hasError = result.Errors.Find(e => e.PropertyName == "CorrelationId") != null;

        Assert.True(hasError == shouldError);
    }
}
