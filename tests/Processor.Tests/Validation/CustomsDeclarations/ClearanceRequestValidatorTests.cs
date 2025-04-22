using AutoFixture;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class ClearanceRequestValidatorTests
{
    private readonly ClearanceRequestValidator _validator = new();

    [Fact]
    public void Validate_Returns_ALVSVAL303_WhenTheClearanceRequestsHaveDuplicateVersions()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().With(cr => cr.ExternalVersion, 1).Create();
        var newClearanceRequest = DataApiClearanceRequestFixture().With(cr => cr.ExternalVersion, 1).Create();
        var mrn = GenerateMrn();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = null,
                Mrn = mrn,
                NewClearanceRequest = newClearanceRequest,
            }
        );

        Assert.NotNull(result.Errors.Find(s => (string)s.CustomState == "ALVSVAL303"));
    }

    [Fact]
    public void Validate_Returns_ALVSVAL303_WhenTheClearanceRequestsDoNotHaveDuplicateVersions()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().With(cr => cr.ExternalVersion, 1).Create();
        var newClearanceRequest = DataApiClearanceRequestFixture().With(cr => cr.ExternalVersion, 2).Create();
        var mrn = GenerateMrn();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = null,
                Mrn = mrn,
                NewClearanceRequest = newClearanceRequest,
            }
        );

        Assert.Null(result.Errors.Find(s => (string)s.CustomState == "ALVSVAL303"));
    }

    [Theory]
    [InlineData(FinalState.CancelledAfterArrival)]
    [InlineData(FinalState.CancelledWhilePreLodged)]
    public void Validate_Returns_ALVSVAL324_WhenClearanceRequestIsCancelled(FinalState finalState)
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newClearanceRequest = DataApiClearanceRequestFixture().Create();
        var mrn = GenerateMrn();
        var existingFinalisation = DataApiFinalisationFixture().With(f => f.FinalState, finalState).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = existingFinalisation,
                Mrn = mrn,
                NewClearanceRequest = newClearanceRequest,
            }
        );

        Assert.NotNull(result.Errors.Find(s => (string)s.CustomState == "ALVSVAL324"));
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL324_WhenClearanceRequestIsNotCancelled()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newClearanceRequest = DataApiClearanceRequestFixture().Create();
        var mrn = GenerateMrn();
        var existingFinalisation = DataApiFinalisationFixture().With(f => f.FinalState, FinalState.Cleared).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = existingFinalisation,
                Mrn = mrn,
                NewClearanceRequest = newClearanceRequest,
            }
        );

        Assert.Null(result.Errors.Find(s => (string)s.CustomState == "ALVSVAL324"));
    }
}
