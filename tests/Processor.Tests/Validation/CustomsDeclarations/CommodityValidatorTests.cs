using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.TestHelper;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class CommodityValidatorTests
{
    private readonly CommodityValidator _validator = new("123");

    [Theory]
    [InlineData(1, false)]
    [InlineData(99999999999.999, false)]
    [InlineData(999999999999.999, true)]
    [InlineData(9999999999.9999, true)]
    public void Validate_Returns_ALVSVAL108_WhenCommodityQuantityIsInvalid(decimal supplementaryUnits, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, SupplementaryUnits = supplementaryUnits };

        var result = _validator.TestValidate(commodity);

        if (shouldError)
        {
            result.ShouldHaveValidationErrorFor(c => c.SupplementaryUnits);
            var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL108");
            Assert.NotNull(error);
            Assert.Contains("Supplementary units format on item number 1 is invalid.", error.ErrorMessage);
            return;
        }

        result.ShouldNotHaveValidationErrorFor(c => c.SupplementaryUnits);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(99999999999.999, false)]
    [InlineData(999999999999.999, true)]
    [InlineData(9999999999.9999, true)]
    public void Validate_Returns_ALVSVAL109_WhenCommodityNetMassIsInvalid(decimal netMass, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, NetMass = netMass };

        var result = _validator.TestValidate(commodity);

        if (shouldError)
        {
            result.ShouldHaveValidationErrorFor(c => c.NetMass);
            var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL109");
            Assert.NotNull(error);
            Assert.Contains("Net mass format on item number 1 is invalid.", error.ErrorMessage);
            return;
        }

        result.ShouldNotHaveValidationErrorFor(c => c.NetMass);
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL308_WhenNoDocumentsAreProvided()
    {
        var commodity = new Commodity { ItemNumber = 1 };

        var result = _validator.TestValidate(commodity);

        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL308");

        Assert.Null(error);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL317_WhenTwoChecksByTheSameAuthorityAreOnTheSameCommodity()
    {
        var commodity = new Commodity
        {
            ItemNumber = 1,
            Checks =
            [
                new CommodityCheck { CheckCode = "H218", DepartmentCode = "HMI" },
                new CommodityCheck { CheckCode = "H220", DepartmentCode = "HMI" },
            ],
        };

        var result = _validator.TestValidate(commodity);

        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL317");

        Assert.NotNull(error);
        Assert.Contains("Item 1 has more than one Item Check defined for the same authority.", error.ErrorMessage);
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL317_WhenThereAreTwoChecks_ButAnIUUCheckCodeIsSpecified()
    {
        var commodity = new Commodity
        {
            ItemNumber = 1,
            Checks =
            [
                new CommodityCheck { CheckCode = "H224", DepartmentCode = "HMI" },
                new CommodityCheck { CheckCode = "H222", DepartmentCode = "HMI" },
            ],
        };

        var result = _validator.TestValidate(commodity);

        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL317");

        Assert.Null(error);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL318_WhenADocumentIsNotProvidedForCommodity()
    {
        var commodity = new Commodity { ItemNumber = 1 };

        var result = _validator.TestValidate(commodity);
        result.ShouldHaveValidationErrorFor(c => c.Documents);

        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL318");

        Assert.NotNull(error);
        Assert.Contains("Item 1 has no document code.", error.ErrorMessage);
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL318_WhenADocumentIsNotProvidedForCommodity_ButItIsAGmrNotification()
    {
        var commodity = new Commodity { ItemNumber = 1, Checks = [new CommodityCheck { CheckCode = "H220" }] };

        var result = _validator.TestValidate(commodity);

        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL318");

        Assert.Null(error);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL320_WhenTheCheckCodesSpecified_ButAreNotRelevantToTheDocumentCodes()
    {
        var commodity = new Commodity
        {
            ItemNumber = 1,
            Checks = [new CommodityCheck { CheckCode = "H222", DepartmentCode = "PHA" }],
            Documents = [new ImportDocument { DocumentCode = "C640" }],
        };

        var result = _validator.TestValidate(commodity);
        result.ShouldHaveValidationErrorFor(c => c.Checks);

        var errors = result.Errors.Where(e => (string)e.CustomState == "ALVSVAL320").ToList();

        Assert.Contains(
            "Document code C640 is not appropriate for the check code requested on ItemNumber 1",
            errors[0].ErrorMessage
        );
    }

    [Fact]
    public void Validate_Returns_ALVSVAL321_WhenCheckCodesAreSpecified_ButNoDocumentCodesAreSpecified()
    {
        var commodity = new Commodity
        {
            ItemNumber = 2,
            Checks = [new CommodityCheck { CheckCode = "H222", DepartmentCode = "PHA" }],
        };

        var result = _validator.TestValidate(commodity);

        var errors = result.Errors.Where(e => (string)e.CustomState == "ALVSVAL321").ToList();

        Assert.Single(errors);
        Assert.Contains("Check code H222 on ItemNumber 2 must have a document code.", errors[0].ErrorMessage);
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL321_WhenCheckCodesAreSpecified_NoDocumentsAreSpecified_ButTheCodeCodeIsForGms()
    {
        var commodity = new Commodity
        {
            ItemNumber = 2,
            Checks = [new CommodityCheck { CheckCode = "H220", DepartmentCode = "PHA" }],
        };

        var result = _validator.TestValidate(commodity);

        var errors = result.Errors.Where(e => (string)e.CustomState == "ALVSVAL321").ToList();

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL328_WhenAnIuuCheckIsSpecifiedButHasNoPoaoCheck()
    {
        var commodity = new Commodity
        {
            ItemNumber = 1,
            Checks = [new CommodityCheck { CheckCode = "H224", DepartmentCode = "PHA" }],
        };

        var result = _validator.TestValidate(commodity);

        var errors = result.Errors.Where(e => (string)e.CustomState == "ALVSVAL328").ToList();

        Assert.Single(errors);
        Assert.Contains("An IUU document has been specified for ItemNumber 1.", errors[0].ErrorMessage);
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL328_WhenIuuCheckANdPoaoCheckIsSpecified()
    {
        var commodity = new Commodity
        {
            ItemNumber = 1,
            Checks =
            [
                new CommodityCheck { CheckCode = "H222", DepartmentCode = "PHA" },
                new CommodityCheck { CheckCode = "H224", DepartmentCode = "PHA" },
            ],
        };

        var result = _validator.TestValidate(commodity);

        var errors = result.Errors.Where(e => (string)e.CustomState == "ALVSVAL328").ToList();

        Assert.Empty(errors);
    }
}
