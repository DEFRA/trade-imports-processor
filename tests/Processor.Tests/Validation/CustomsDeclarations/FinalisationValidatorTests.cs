using AutoFixture;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.Results;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class FinalisationValidatorTests
{
    private readonly FinalisationValidator _validator = new();

    private static ValidationFailure? FindWithErrorCode(ValidationResult result, string errorCode)
    {
        return result.Errors.Find(s => (string)s.CustomState == errorCode);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL401_WhenFinalisationExternalVersionDoesNotMatchTheClearanceRequestExternalVersion()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().With(c => c.ExternalVersion, 2).Create();
        var newFinalisation = DataApiFinalisationFixture()
            .With(f => f.ExternalVersion, 1)
            .With(f => f.FinalState, FinalStateValues.ReleasedToKingsWarehouse.ToString)
            .Create();
        var mrn = GenerateMrn();

        var result = _validator.Validate(
            new FinalisationValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                NewFinalisation = newFinalisation,
                Mrn = mrn,
            }
        );

        var error = FindWithErrorCode(result, "ALVSVAL401");

        Assert.NotNull(error);
        Assert.Contains(
            $"The finalised state was received for EntryReference {mrn} EntryVersionNumber 1.",
            error.ErrorMessage
        );
    }

    [Fact]
    public void Validate_Returns_ALVSVAL402_WhenTheFinalStateIsUnknown()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newFinalisation = DataApiFinalisationFixture().With(f => f.FinalState, "999").Create();

        var result = _validator.Validate(
            new FinalisationValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = null,
                NewFinalisation = newFinalisation,
                Mrn = GenerateMrn(),
            }
        );

        var error = FindWithErrorCode(result, "ALVSVAL402");

        Assert.NotNull(error);
        Assert.Contains("The FinalState 999 is invalid.", error.ErrorMessage);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL403_WhenExistingFinalisationIsCancelledAndNewFinalisationIsNotCancelled()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newFinalisation = DataApiFinalisationFixture()
            .With(f => f.ExternalVersion, 2)
            .With(f => f.FinalState, FinalStateValues.Cleared.ToString)
            .Create();
        var existingFinalisation = DataApiFinalisationFixture()
            .With(f => f.FinalState, FinalStateValues.CancelledWhilePreLodged.ToString)
            .Create();
        var mrn = GenerateMrn();

        var result = _validator.Validate(
            new FinalisationValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = existingFinalisation,
                NewFinalisation = newFinalisation,
                Mrn = mrn,
            }
        );

        var error = FindWithErrorCode(result, "ALVSVAL403");

        Assert.NotNull(error);
        Assert.Contains(
            $"The final state was received for EntryReference {mrn} EntryVersionNumber 2 but the import declaration was cancelled.",
            error.ErrorMessage
        );
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL403_WhenExistingFinalisationIsNotCancelled()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newFinalisation = DataApiFinalisationFixture()
            .With(f => f.ExternalVersion, 2)
            .With(f => f.FinalState, FinalStateValues.Cleared.ToString)
            .Create();
        var existingFinalisation = DataApiFinalisationFixture()
            .With(f => f.FinalState, FinalStateValues.Seized.ToString)
            .Create();
        var mrn = GenerateMrn();

        var result = _validator.Validate(
            new FinalisationValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = existingFinalisation,
                NewFinalisation = newFinalisation,
                Mrn = mrn,
            }
        );

        var error = FindWithErrorCode(result, "ALVSVAL403");

        Assert.Null(error);
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL403_WhenBothExistingAndNewFinalisationsAreCancelled()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newFinalisation = DataApiFinalisationFixture()
            .With(f => f.ExternalVersion, 2)
            .With(f => f.FinalState, FinalStateValues.CancelledAfterArrival.ToString)
            .Create();
        var existingFinalisation = DataApiFinalisationFixture()
            .With(f => f.FinalState, FinalStateValues.CancelledWhilePreLodged.ToString)
            .Create();
        var mrn = GenerateMrn();

        var result = _validator.Validate(
            new FinalisationValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = existingFinalisation,
                NewFinalisation = newFinalisation,
                Mrn = mrn,
            }
        );

        var error = FindWithErrorCode(result, "ALVSVAL403");

        Assert.Null(error);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL501_WhenACancelledFinalisationHasAlreadyBeenReceivedAndANewCancellationIsSent()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newFinalisation = DataApiFinalisationFixture()
            .With(f => f.FinalState, FinalStateValues.CancelledAfterArrival.ToString)
            .Create();
        var existingFinalisation = DataApiFinalisationFixture()
            .With(f => f.FinalState, FinalStateValues.CancelledAfterArrival.ToString)
            .Create();
        var mrn = GenerateMrn();

        var result = _validator.Validate(
            new FinalisationValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = existingFinalisation,
                NewFinalisation = newFinalisation,
                Mrn = mrn,
            }
        );

        var error = FindWithErrorCode(result, "ALVSVAL501");

        Assert.NotNull(error);
        Assert.Contains(
            $"An attempt to cancel EntryReference {mrn} EntryVersionNumber 1 was made but the import declaration was cancelled.",
            error.ErrorMessage
        );
    }

    [Fact]
    public void Validate_Returns_ALVSVAL506_WhenANewFinalisationAttemptsToCancelButItIsOutOfDate()
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newFinalisation = DataApiFinalisationFixture()
            .With(f => f.ExternalVersion, 2)
            .With(f => f.FinalState, FinalStateValues.CancelledAfterArrival.ToString)
            .Create();
        var existingFinalisation = DataApiFinalisationFixture()
            .With(f => f.FinalState, FinalStateValues.Seized.ToString)
            .Create();
        var mrn = GenerateMrn();

        var result = _validator.Validate(
            new FinalisationValidatorInput
            {
                ExistingClearanceRequest = existingClearanceRequest,
                ExistingFinalisation = existingFinalisation,
                NewFinalisation = newFinalisation,
                Mrn = mrn,
            }
        );

        var error = FindWithErrorCode(result, "ALVSVAL506");

        Assert.NotNull(error);
        Assert.Contains(
            $"The import declaration was received as a cancellation. The EntryReference {mrn} EntryVersionNumber 2 have already been replaced by a later version.",
            error.ErrorMessage
        );
    }
}
