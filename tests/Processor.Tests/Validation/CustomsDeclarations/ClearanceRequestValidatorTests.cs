using AutoFixture;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;
using ClearanceRequest = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.ClearanceRequest;

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
    [InlineData(FinalStateValues.CancelledAfterArrival)]
    [InlineData(FinalStateValues.CancelledWhilePreLodged)]
    public void Validate_Returns_ALVSVAL324_WhenClearanceRequestIsCancelled(FinalStateValues finalState)
    {
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var newClearanceRequest = DataApiClearanceRequestFixture().Create();
        var mrn = GenerateMrn();
        var existingFinalisation = DataApiFinalisationFixture().With(f => f.FinalState, finalState.ToString).Create();

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
        var existingFinalisation = DataApiFinalisationFixture()
            .With(f => f.FinalState, FinalStateValues.Cleared.ToString)
            .Create();

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

    [Theory]
    [InlineData(null, false)]
    [InlineData(1, false)]
    [InlineData(99, false)]
    [InlineData(0, true)]
    [InlineData(100, true)]
    private void Validate_PreviousExternalVersion_ERR005(int? previousExternalVersion, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.PreviousExternalVersion, previousExternalVersion)
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR005") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345678901234567890123456789012345", false)]
    [InlineData("123456789012345678901234567890123456", true)]
    [InlineData("1234567890123456789012345678901234567890", true)]
    private void Validate_DeclarationUcr_ERR006(string? declarationUcr, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.DeclarationUcr, declarationUcr).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR006") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("1", false)]
    [InlineData("12", false)]
    [InlineData("123", false)]
    [InlineData("1234", true)]
    [InlineData("12345", true)]
    private void Validate_DeclarationPartNumber_ERR007(string? declarationPartNumber, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.DeclarationPartNumber, declarationPartNumber)
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR007") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData("S", false)]
    [InlineData("F", false)]
    [InlineData("T", true)]
    [InlineData("X", true)]
    [InlineData(null, true)]
    [InlineData("", true)]
    private void Validate_DeclarationType_ERR008(string? declarationType, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.DeclarationType, declarationType)
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR008") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("123456789012345678", false)]
    [InlineData("1234567890123456789", true)]
    [InlineData("12345678901234567890", true)]
    private void Validate_SubmitterTurn_ERR010(string? submitterTurn, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.SubmitterTurn, submitterTurn).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR010") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("123456789012345678", false)]
    [InlineData("1234567890123456789", true)]
    [InlineData("12345678901234567890", true)]
    private void Validate_DeclarantId_ERR011(string? declarantId, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.DeclarantId, declarantId).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR011") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Valid Company Name Maximum Length", false)]
    [InlineData("This Company Name Is Too Long For Field", true)]
    [InlineData("This Company Name Is Far Too Long For The Field", true)]
    private void Validate_DeclarantName_ERR012(string? declarantName, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.DeclarantName, declarantName).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR012") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("GB", false)]
    [InlineData("FR", false)]
    [InlineData("US", false)]
    [InlineData("G", true)]
    [InlineData("GBR", true)]
    [InlineData("USA", true)]
    private void Validate_DispatchCountryCode_ERR013(string? dispatchCountryCode, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.DispatchCountryCode, dispatchCountryCode)
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR013") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("MNCMANMNC", false)]
    [InlineData("GBLHRAIRPORT001", false)]
    [InlineData("12345678901234567", false)]
    [InlineData("123456789012345678", true)]
    [InlineData("GBLHRAIRPORTLOCATION", true)]
    private void Validate_GoodsLocationCode_ERR014(string? goodsLocationCode, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.GoodsLocationCode, goodsLocationCode)
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR014") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("GB123456789012345678901234567890123", false)]
    [InlineData("12345678901234567890123456789012345", false)]
    [InlineData("GB1234567890123456789012345678901234", true)]
    [InlineData("123456789012345678901234567890123456", true)]
    private void Validate_MasterUcr_ERR015(string? masterUcr, bool shouldError)
    {
        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.MasterUcr, masterUcr).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var hasError = FindWithErrorCode(result, "ERR015") != null;
        Assert.True(hasError == shouldError);
    }

#pragma warning disable xUnit1045
    [Theory, ClassData(typeof(ClearanceRequestValidatorTestData))]
#pragma warning restore xUnit1045
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
            Add(
                DataApiClearanceRequestFixture().With(d => d.DeclarationPartNumber, "123").Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationPartNumber", false)
            );
            Add(
                DataApiClearanceRequestFixture().With(d => d.DeclarationPartNumber, "1234").Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationPartNumber", true)
            );
            Add(
                DataApiClearanceRequestFixture().Without(d => d.DeclarationPartNumber).Create(),
                new ExpectedResult("NewClearanceRequest.DeclarationPartNumber", false)
            );
        }
    }
}
