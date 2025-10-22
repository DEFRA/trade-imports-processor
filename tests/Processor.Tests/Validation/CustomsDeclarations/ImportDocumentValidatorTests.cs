using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.TestHelper;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class ImportDocumentValidatorTests
{
    private readonly ImportDocumentValidator _validator = new(1, "123");

    [Fact]
    public void Validate_Returns_ALVSVAL308_WhenADocumentCodeIsInvalid()
    {
        var importDocument = new ImportDocument { DocumentCode = "UNKNOWN" };

        var result = _validator.TestValidate(importDocument);
        result.ShouldHaveValidationErrorFor(doc => doc.DocumentCode);

        var error = result.Errors.Find(e => (string)e.CustomState == "ALVSVAL308");

        Assert.NotNull(error);
        Assert.Contains("DocumentCode UNKNOWN on item number 1 is invalid.", error.ErrorMessage);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("CHEDD.GB.2024.5555554", false)]
    [InlineData("CHEDPP.GB.2024.5194492", false)]
    [InlineData("Short-Reference", false)]
    [InlineData("This-is-exactly-thirty-five-chars", false)]
    [InlineData("This-reference-is-way-too-long-exceeding-the-thirty-five-character-limit", true)]
    public void Validate_DocumentReference_ERR025(string? documentReference, bool shouldError)
    {
        var importDocument = new ImportDocument
        {
            DocumentReference = documentReference != null ? new ImportDocumentReference(documentReference) : null,
        };

        var result = _validator.TestValidate(importDocument);

        var hasError = result.Errors.Find(e => (string)e.CustomState == "ERR025") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("P", false)]
    [InlineData("X", false)]
    [InlineData("A", false)]
    [InlineData("PP", true)]
    [InlineData("ABC", true)]
    public void Validate_DocumentControl_ERR026(string? documentControl, bool shouldError)
    {
        var importDocument = new ImportDocument { DocumentControl = documentControl };

        var result = _validator.TestValidate(importDocument);

        var hasError = result.Errors.Find(e => (string)e.CustomState == "ERR026") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("AE", false)]
    [InlineData("XX", false)]
    [InlineData("P", false)]
    [InlineData("AEE", true)]
    [InlineData("ABC", true)]
    public void Validate_DocumentStatus_ERR027(string? documentStatus, bool shouldError)
    {
        var importDocument = new ImportDocument { DocumentStatus = documentStatus };

        var result = _validator.TestValidate(importDocument);

        var hasError = result.Errors.Find(e => (string)e.CustomState == "ERR027") != null;
        Assert.True(hasError == shouldError);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(0d, false)]
    [InlineData(1.5d, false)]
    [InlineData(99999999999.999d, false)]
    [InlineData(999999999999.999d, true)]
    [InlineData(9999999999.9999d, true)]
    public void Validate_DocumentQuantity_ERR028(double? documentQuantity, bool shouldError)
    {
        var importDocument = new ImportDocument { DocumentQuantity = (decimal?)documentQuantity };

        var result = _validator.TestValidate(importDocument);

        var hasError = result.Errors.Find(e => (string)e.CustomState == "ERR028") != null;
        Assert.True(hasError == shouldError);
    }

#pragma warning disable xUnit1045
    [Theory, ClassData(typeof(ImportDocumentValidatorTestData))]
#pragma warning restore xUnit1045
    public void TheoryTests(ImportDocument model, ExpectedResult expectedResult)
    {
        var result = _validator.TestValidate(model);

        if (expectedResult.HasValidationError)
        {
            result.ShouldHaveValidationErrorFor(expectedResult.PropertyName);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(expectedResult.PropertyName);
        }
    }

    public class ImportDocumentValidatorTestData : TheoryData<ImportDocument, ExpectedResult>
    {
        public ImportDocumentValidatorTestData()
        {
            Add(
                new ImportDocument { DocumentCode = "N002" },
                new ExpectedResult(nameof(ImportDocument.DocumentCode), false)
            );
            Add(
                new ImportDocument { DocumentCode = null },
                new ExpectedResult(nameof(ImportDocument.DocumentCode), true)
            );

            Add(
                new ImportDocument { DocumentStatus = "AE" },
                new ExpectedResult(nameof(ImportDocument.DocumentStatus), false)
            );
            Add(
                new ImportDocument { DocumentStatus = "AEE" },
                new ExpectedResult(nameof(ImportDocument.DocumentStatus), true)
            );
            Add(
                new ImportDocument { DocumentStatus = null },
                new ExpectedResult(nameof(ImportDocument.DocumentStatus), true)
            );

            Add(
                new ImportDocument { DocumentControl = "P" },
                new ExpectedResult(nameof(ImportDocument.DocumentControl), false)
            );
            Add(
                new ImportDocument { DocumentControl = "PP" },
                new ExpectedResult(nameof(ImportDocument.DocumentControl), true)
            );
            Add(
                new ImportDocument { DocumentControl = null },
                new ExpectedResult(nameof(ImportDocument.DocumentControl), true)
            );
        }
    }
}
