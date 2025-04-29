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

    [Theory, ClassData(typeof(ImportDocumentValidatorTestData))]
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
                new ImportDocument { DocumentCode = "C633" },
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
