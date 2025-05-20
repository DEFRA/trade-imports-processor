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
}
