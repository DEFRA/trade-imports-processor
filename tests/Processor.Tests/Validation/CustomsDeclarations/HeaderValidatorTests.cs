using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.Results;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class HeaderValidatorTests
{
    private readonly HeaderValidator _validator = new("123");

    private static ValidationFailure? FindWithErrorCode(ValidationResult result, string errorCode)
    {
        return result.Errors.Find(s => (string)s.CustomState == errorCode);
    }

    [Fact]
    public void Validate_Returns_ALVSVAL153_WhenEntryVersionNumberNotSet()
    {
        var header = new Header { EntryReference = "ref" };

        var result = _validator.Validate(header);

        Assert.NotNull(FindWithErrorCode(result, "ALVSVAL153"));
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("24GBDEJ9V2OD0BHAR1", false)]
    [InlineData("12AB123456789012345", true)]
    [InlineData("12ab123456789012345", true)]
    [InlineData("12AB12345678901234", false)]
    [InlineData("12AB1234567890123456", true)]
    [InlineData("1AAB123456789012345", true)]
    [InlineData("A2AB123456789012345", true)]
    [InlineData("12A3123456789012345", true)]
    [InlineData("123B123456789012345", true)]
    [InlineData("AB123456789012345678", true)]
    private void Validate_EntryReference_ERR003(string entryReference, bool shouldError)
    {
        var header = new Header { EntryReference = entryReference, EntryVersionNumber = 1 };

        var result = _validator.Validate(header);
        var hasError = FindWithErrorCode(result, "ERR003") != null;

        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(-1, true)]
    [InlineData(100, true)]
    [InlineData(1, false)]
    [InlineData(50, false)]
    [InlineData(99, false)]
    public void Validate_EntryVersionNumber_ERR004(int entryVersionNumber, bool shouldHaveError)
    {
        var header = new Header { EntryReference = "24GBDEJ9V2OD0BHAR1", EntryVersionNumber = entryVersionNumber };

        var result = _validator.Validate(header);

        var hasError = FindWithErrorCode(result, "ERR004") != null;
        Assert.True(hasError == shouldHaveError);
    }
}
