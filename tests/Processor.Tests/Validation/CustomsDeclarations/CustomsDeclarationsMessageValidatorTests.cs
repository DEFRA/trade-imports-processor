using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using Elastic.CommonSchema;
using FluentValidation.TestHelper;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class CustomsDeclarationsMessageValidatorTests
{
    private readonly CustomsDeclarationsMessageValidator _validator = new();

    [Theory, ClassData(typeof(CustomsDeclarationsMessageValidatorTestData))]
    public void TheoryTests(CustomsDeclarationsMessage customsDeclarationsMessage, ExpectedResult expectedResult)
    {
        var result = _validator.TestValidate(customsDeclarationsMessage);

        if (expectedResult.HasValidationError)
        {
            result.ShouldHaveValidationErrorFor(expectedResult.PropertyName);
            return;
        }

        result.ShouldNotHaveValidationErrorFor(expectedResult.PropertyName);
    }

    public class CustomsDeclarationsMessageValidatorTestData : TheoryData<CustomsDeclarationsMessage, ExpectedResult>
    {
        public CustomsDeclarationsMessageValidatorTestData()
        {
            Add(
                new CustomsDeclarationsMessage
                {
                    Header = new Header { EntryReference = "15GB1245fst7s8g9s4" },
                    ServiceHeader = ServiceHeaderFixture().Create(),
                },
                new ExpectedResult("Header.EntryReference", false)
            );
            Add(
                new CustomsDeclarationsMessage
                {
                    Header = new Header { EntryReference = "invalid" },
                    ServiceHeader = ServiceHeaderFixture().Create(),
                },
                new ExpectedResult("Header.EntryReference", true)
            );
            Add(
                new CustomsDeclarationsMessage
                {
                    Header = new Header { EntryReference = "15GB1245fst7s8g9s4", EntryVersionNumber = 2 },
                    ServiceHeader = ServiceHeaderFixture().Create(),
                },
                new ExpectedResult("Header.EntryVersionNumber", false)
            );
            Add(
                new CustomsDeclarationsMessage
                {
                    Header = new Header { EntryReference = "15GB1245fst7s8g9s4", EntryVersionNumber = -1 },
                    ServiceHeader = ServiceHeaderFixture().Create(),
                },
                new ExpectedResult("Header.EntryVersionNumber", true)
            );
            Add(
                new CustomsDeclarationsMessage
                {
                    Header = new Header { EntryReference = "15GB1245fst7s8g9s4", EntryVersionNumber = 100 },
                    ServiceHeader = ServiceHeaderFixture().Create(),
                },
                new ExpectedResult("Header.EntryVersionNumber", true)
            );
        }
    }
}
