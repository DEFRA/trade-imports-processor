using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.Results;
using FluentValidation.TestHelper;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class CommodityValidatorTests
{
    private readonly CommodityValidator _validator = new("123");

    private static ValidationFailure? FindWithErrorCode(ValidationResult result, string errorCode)
    {
        return result.Errors.Find(s => (string)s.CustomState == errorCode);
    }

    [Theory]
    [InlineData(0, false)]
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
    [InlineData(0, false)]
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
                new CommodityCheck { CheckCode = "H220", DepartmentCode = "HMI-GMS" },
                new CommodityCheck { CheckCode = "H220", DepartmentCode = "HMI-GMS" },
            ],
        };

        var result = _validator.TestValidate(commodity);

        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL317");

        Assert.NotNull(error);
        Assert.Contains("Item 1 has more than one Item Check defined for the same authority.", error.ErrorMessage);
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL317_WhenTwoCheckCodesAreFromTheSameDepartment_ButAreADifferentImportType()
    {
        var commodity = new Commodity
        {
            ItemNumber = 1,
            Checks =
            [
                new CommodityCheck { CheckCode = "H222", DepartmentCode = "PHA-POAO" },
                new CommodityCheck { CheckCode = "H223", DepartmentCode = "PHA-FNAO" },
            ],
        };

        var result = _validator.TestValidate(commodity);

        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL317");

        Assert.Null(error);
    }

    [Fact]
    public void Validate_DoesNotReturn_ALVSVAL317_WhenThereAreTwoChecks_ButAnIUUCheckCodeIsSpecified()
    {
        var commodity = new Commodity
        {
            ItemNumber = 1,
            Checks =
            [
                new CommodityCheck { CheckCode = "H224", DepartmentCode = "PHA-IUU" },
                new CommodityCheck { CheckCode = "H222", DepartmentCode = "PHA-POAO" },
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

    [Theory]
    [InlineData(null, true)]
    [InlineData(0, true)]
    [InlineData(1000, true)]
    [InlineData(1, false)]
    [InlineData(500, false)]
    [InlineData(999, false)]
    public void Validate_ItemNumber_ERR016(int? itemNumber, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = itemNumber };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR016") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("1234567", false)]
    [InlineData("12345678", true)]
    [InlineData("ABC1234", false)]
    public void Validate_CustomsProcedureCode_ERR017(string? customsProcedureCode, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, CustomsProcedureCode = customsProcedureCode };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR017") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("1234567890", false)]
    [InlineData("123456789", true)]
    [InlineData("12345678901", true)]
    [InlineData("ABCDEFGHIJ", true)]
    [InlineData("123456789A", true)]
    public void Validate_TaricCommodityCode_ERR018(string? taricCommodityCode, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, TaricCommodityCode = taricCommodityCode };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR018") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Horses", false)]
    [InlineData("Extremely Large Vegetables", false)]
    [InlineData("Significantly Large Sausages", false)]
    [InlineData("Fresh or chilled beef cuts with bone in, other than carcasses and half-carcasses", false)]
    [InlineData(
        "This is an extremely long goods description that exceeds the maximum allowed length of 280 characters for validation purposes and should trigger an error when processed by the validation rules that have been implemented to ensure data quality and compliance with the system requirements that govern the acceptable length of goods descriptions in the customs declaration processing system",
        true
    )]
    public void Validate_GoodsDescription_ERR019(string? goodsDescription, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, GoodsDescription = goodsDescription };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR019") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("123456789012345678", false)]
    [InlineData("1234567890123456789", true)]
    [InlineData("12345678901234567890", true)]
    public void Validate_ConsigneeId_ERR020(string? consigneeId, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, ConsigneeId = consigneeId };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR020") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("ABC Trading Company Ltd", false)]
    [InlineData("Smith & Sons Import Export Ltd", false)]
    [InlineData("This Company Name Is Way Too Long For Field", true)]
    public void Validate_ConsigneeName_ERR021(string? consigneeName, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, ConsigneeName = consigneeName };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR021") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    public void Validate_NetMass_ERR022(decimal? netMass, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, NetMass = netMass };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR022") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(0d, false)]
    [InlineData(1.5d, false)]
    [InlineData(99999999999.999d, false)]
    [InlineData(999999999999.999d, true)]
    [InlineData(9999999999.9999d, true)]
    public void Validate_ThirdQuantity_ERR023(double? thirdQuantity, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, ThirdQuantity = (decimal?)thirdQuantity };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR023") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("GB", false)]
    [InlineData("US", false)]
    [InlineData("FR", false)]
    [InlineData("G", true)]
    [InlineData("GBR", true)]
    [InlineData("USA", true)]
    public void Validate_OriginCountryCode_ERR024(string? originCountryCode, bool shouldError)
    {
        var commodity = new Commodity { ItemNumber = 1, OriginCountryCode = originCountryCode };

        var result = _validator.Validate(commodity);

        var hasError = FindWithErrorCode(result, "ERR024") != null;
        Assert.True(hasError == shouldError);
    }
}
