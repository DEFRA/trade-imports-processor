using AutoFixture;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;
using CommodityCheck = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.CommodityCheck;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class ClearanceRequestValidatorTests
{
    private readonly ClearanceRequestValidator _validator = new();

    private static ValidationFailure? FindWithErrorCode(ValidationResult result, string errorCode)
    {
        return result.Errors.Find(s => (string)s.CustomState == errorCode);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL152_WhenEntryVersionNumberNot1_AndPreviousVersionNumberNotSet()
    {
        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.ExternalVersion, 2)
            .Without(c => c.PreviousExternalVersion)
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        Assert.NotNull(FindWithErrorCode(result, "ALVSVAL152"));
    }

    [Fact]
    public void Validate_Returns_ALVSVAL153_WhenEntryVersionNumberNotSet()
    {
        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.ExternalVersion, (int?)null).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        Assert.NotNull(FindWithErrorCode(result, "ALVSVAL153"));
    }

    [Fact]
    public void Validate_Returns_ALVSVAL164_WhenAnItemNumberAppearsMoreThanOnce()
    {
        Commodity[] commodities = [new() { ItemNumber = 1 }, new() { ItemNumber = 2 }, new() { ItemNumber = 2 }];
        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.Commodities, commodities).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var errors = FindWithErrorCode(result, "ALVSVAL164");

        Assert.NotNull(errors);
        Assert.Contains("Item number 2 is invalid as it appears more than once.", errors.ErrorMessage);
    }

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
                Mrn = mrn,
                NewClearanceRequest = newClearanceRequest,
            }
        );

        Assert.NotNull(FindWithErrorCode(result, "ALVSVAL303"));
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
                Mrn = mrn,
                NewClearanceRequest = newClearanceRequest,
            }
        );

        Assert.Null(FindWithErrorCode(result, "ALVSVAL303"));
    }

    [Fact]
    public void Validate_Returns_ALVSVAL313_WhenDeclarationUCRIsNull()
    {
        var newClearanceRequest = DataApiClearanceRequestFixture().Without(c => c.DeclarationUcr).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var errors = FindWithErrorCode(result, "ALVSVAL313");

        Assert.NotNull(errors);
        Assert.Contains("The DeclarationUCR field must have a value.", errors.ErrorMessage);
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

        Assert.NotNull(FindWithErrorCode(result, "ALVSVAL324"));
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

        Assert.Null(FindWithErrorCode(result, "ALVSVAL324"));
    }

    [Fact]
    public void Validate_Returns_ALVSVAL326_WhenThePreviousVersionIsGreaterThanCurrent()
    {
        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.ExternalVersion, 1)
            .With(c => c.PreviousExternalVersion, 2)
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        Assert.NotNull(FindWithErrorCode(result, "ALVSVAL326"));
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL326_WhenThePreviousVersionIsNull()
    {
        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.ExternalVersion, 1)
            .Without(c => c.PreviousExternalVersion)
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        Assert.Null(FindWithErrorCode(result, "ALVSVAL326"));
    }

    [Theory, ClassData(typeof(ClearanceRequestValidatorTestData))]
    public void TheoryTests(ClearanceRequest clearanceRequest, ExpectedResult expectedResult)
    {
        var result = _validator.TestValidate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = clearanceRequest }
        );

        if (expectedResult.HasValidationError)
        {
            result.ShouldHaveValidationErrorFor(expectedResult.PropertyName);
            return;
        }

        result.ShouldNotHaveValidationErrorFor(expectedResult.PropertyName);
    }

    public class ClearanceRequestValidatorTestData : TheoryData<ClearanceRequest, ExpectedResult>
    {
        public ClearanceRequestValidatorTestData()
        {
            Add(
                DataApiClearanceRequestFixture()
                    .With(d => d.DeclarationUcr, "1234567891234567891233fghytfcdsertgy")
                    .Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationUcr", true)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DeclarationUcr, "valid").Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationUcr", false)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DeclarationType, "F").Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationType", false)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DeclarationType, "S").Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationType", false)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DeclarationType, "T").Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationType", true)
            );
            Add(
                DataApiClearanceRequestFixture().Without(d => d.DeclarationType).Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationType", true)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DeclarantId, "valid").Create(),
                new ExpectedResult("NewClearanceRequest.DeclarantId", false)
            );
            Add(
                DataApiClearanceRequestFixture()
                    .With(d => d.DeclarantId, "1234567891234567891233fghytfcdsertgy")
                    .Create(),
                new ExpectedResult("NewClearanceRequest.DeclarantId", true)
            );
            Add(
                DataApiClearanceRequestFixture().Without(d => d.DeclarantId).Create(),
                new ExpectedResult("NewClearanceRequest.DeclarantId", true)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DeclarantName, "valid").Create(),
                new ExpectedResult("NewClearanceRequest.DeclarantName", false)
            );
            Add(
                DataApiClearanceRequestFixture()
                    .With(d => d.DeclarantName, "1234567891234567891233fghytfcdsertgy")
                    .Create(),
                new ExpectedResult("NewClearanceRequest.DeclarantName", true)
            );
            Add(
                DataApiClearanceRequestFixture().Without(d => d.DeclarantName).Create(),
                new ExpectedResult("NewClearanceRequest.DeclarantName", true)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DispatchCountryCode, "GB").Create(),
                new ExpectedResult("NewClearanceRequest.DispatchCountryCode", false)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DispatchCountryCode, "T").Create(),
                new ExpectedResult("NewClearanceRequest.DispatchCountryCode", true)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DispatchCountryCode, "GBB").Create(),
                new ExpectedResult("NewClearanceRequest.DispatchCountryCode", true)
            );
            Add(
                DataApiClearanceRequestFixture().Without(d => d.DispatchCountryCode).Create(),
                new ExpectedResult("NewClearanceRequest.DispatchCountryCode", true)
            );
        }
    }
}
