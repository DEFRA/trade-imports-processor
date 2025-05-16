using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.TestHelper;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class CheckValidatorTests
{
    private readonly CheckValidator _validator = new(1, "123");

    [Fact]
    public void Validate_Returns_ALVSVAL311_WhenACommodityCheckHasNoCheckCode()
    {
        var check = new CommodityCheck();

        var result = _validator.TestValidate(check);
        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL311");

        Assert.NotNull(error);
        Assert.Contains("The CheckCode field on item number 1 must have a value.", error.ErrorMessage);
    }

#pragma warning disable xUnit1045
    [Theory, ClassData(typeof(CheckValidatorTestData))]
#pragma warning restore xUnit1045
    public void TheoryTests(CommodityCheck check, ExpectedResult expectedResult)
    {
        var result = _validator.TestValidate(check);

        if (expectedResult.HasValidationError)
        {
            result.ShouldHaveValidationErrorFor(expectedResult.PropertyName);
            return;
        }

        result.ShouldNotHaveValidationErrorFor(expectedResult.PropertyName);
    }

    public class CheckValidatorTestData : TheoryData<CommodityCheck, ExpectedResult>
    {
        public CheckValidatorTestData()
        {
            Add(
                new CommodityCheck { DepartmentCode = "test" },
                new ExpectedResult(nameof(CommodityCheck.DepartmentCode), false)
            );
            Add(
                new CommodityCheck { DepartmentCode = "qwertyuip" },
                new ExpectedResult(nameof(CommodityCheck.DepartmentCode), true)
            );
            Add(
                new CommodityCheck { DepartmentCode = null },
                new ExpectedResult(nameof(CommodityCheck.DepartmentCode), true)
            );

            Add(new CommodityCheck { CheckCode = "test" }, new ExpectedResult(nameof(CommodityCheck.CheckCode), false));
            Add(
                new CommodityCheck { CheckCode = "qwerty" },
                new ExpectedResult(nameof(CommodityCheck.CheckCode), true)
            );
            Add(new CommodityCheck { CheckCode = null }, new ExpectedResult(nameof(CommodityCheck.CheckCode), true));
        }
    }
}
