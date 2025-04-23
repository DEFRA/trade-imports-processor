using AutoFixture;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.Results;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class ClearanceRequestValidatorTests
{
    private readonly ClearanceRequestValidator _validator = new();

    private static ValidationFailure? FindWithErrorCode(ValidationResult result, string errorCode)
    {
        return result.Errors.Find(s => (string)s.CustomState == errorCode);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(99999999999.999, false)]
    [InlineData(999999999999.999, true)]
    [InlineData(9999999999.9999, true)]
    public void Validate_Returns_ALVSVAL108_WhenCommodityQuantityIsInvalid(decimal supplementaryUnits, bool shouldError)
    {
        Commodity[] commodities = [new() { ItemNumber = 1, SupplementaryUnits = supplementaryUnits }];

        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.Commodities, commodities).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var errors = FindWithErrorCode(result, "ALVSVAL108");

        if (shouldError)
        {
            Assert.NotNull(errors);
            Assert.Contains("Supplementary units format on item number 1 is invalid.", errors.ErrorMessage);
            return;
        }

        Assert.Null(errors);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(99999999999.999, false)]
    [InlineData(999999999999.999, true)]
    [InlineData(9999999999.9999, true)]
    public void Validate_Returns_ALVSVAL109_WhenCommodityNetMassIsInvalid(decimal netMass, bool shouldError)
    {
        Commodity[] commodities = [new() { ItemNumber = 1, NetMass = netMass }];

        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.Commodities, commodities).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var errors = FindWithErrorCode(result, "ALVSVAL109");

        if (shouldError)
        {
            Assert.NotNull(errors);
            Assert.Contains("Net mass format on item number 1 is invalid.", errors.ErrorMessage);
            return;
        }

        Assert.Null(errors);
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
                ExistingFinalisation = null,
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
                ExistingFinalisation = null,
                Mrn = mrn,
                NewClearanceRequest = newClearanceRequest,
            }
        );

        Assert.Null(FindWithErrorCode(result, "ALVSVAL303"));
    }

    [Fact]
    public void Validate_Returns_ALVSVAL308_WhenADocumentCodeIsInvalidOnACommodity()
    {
        var validDocumentCodes = _validator.ValidDocumentCodes.ToList();

        var commodities = new List<Commodity>
        {
            new() { ItemNumber = 1, Documents = [new ImportDocument { DocumentCode = "UNKNOWN" }] },
            new()
            {
                ItemNumber = 2,
                Documents =
                [
                    new ImportDocument { DocumentCode = validDocumentCodes[0] },
                    new ImportDocument { DocumentCode = "UNKNOWN2" },
                ],
            },
            new() { ItemNumber = 3 },
        };

        for (var i = 0; i < _validator.ValidDocumentCodes.Count; i++)
        {
            commodities.Add(
                new Commodity
                {
                    ItemNumber = i + 3,
                    Documents = [new ImportDocument { DocumentCode = validDocumentCodes[i] }],
                }
            );
        }

        var newClearanceRequest = DataApiClearanceRequestFixture()
            .With(c => c.Commodities, commodities.ToArray())
            .Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var errors = result.Errors.Where(e => (string)e.CustomState == "ALVSVAL308").ToList();

        Assert.Equal(2, errors.Count);
        Assert.Contains("DocumentCode UNKNOWN on item number 1 is invalid.", errors[0].ErrorMessage);
        Assert.Contains("DocumentCode UNKNOWN2 on item number 2 is invalid.", errors[1].ErrorMessage);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL311_WhenACommodityHasACheckWithoutACheckCode()
    {
        Commodity[] commodities =
        [
            new() { ItemNumber = 1, Checks = [new CommodityCheck { DepartmentCode = "ABCD" }] },
            new() { ItemNumber = 2 },
        ];
        var newClearanceRequest = DataApiClearanceRequestFixture().With(c => c.Commodities, commodities).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var errors = FindWithErrorCode(result, "ALVSVAL311");

        Assert.NotNull(errors);
        Assert.Contains("The CheckCode field on item number 1 must have a value.", errors.ErrorMessage);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL313_WhenDeclarationUCRIsNull()
    {
        var newClearanceRequest = DataApiClearanceRequestFixture().Without(c => c.DeclarationUcr).Create();

        var result = _validator.Validate(
            new ClearanceRequestValidatorInput { Mrn = GenerateMrn(), NewClearanceRequest = newClearanceRequest }
        );

        var errors = FindWithErrorCode(result, "ALVSVAL313");

        // TO-DO: This error message is being reviewed
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
}
