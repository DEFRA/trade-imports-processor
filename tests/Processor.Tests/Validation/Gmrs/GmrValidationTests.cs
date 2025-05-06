using Defra.TradeImportsDataApi.Domain.Gvms;
using Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.Gmrs;
using FluentValidation.TestHelper;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.Gmrs;

public class GmrValidationTests
{
    private readonly GmrValidator _validator = new();

    [Theory, ClassData(typeof(GmrValidationTestData))]
    public void TheoryTests(Gmr gmr, ExpectedResult expectedResult)
    {
        var result = _validator.TestValidate(gmr);

        if (expectedResult.HasValidationError)
        {
            result.ShouldHaveValidationErrorFor(expectedResult.PropertyName);
            return;
        }

        result.ShouldNotHaveValidationErrorFor(expectedResult.PropertyName);
    }

    public class GmrValidationTestData : TheoryData<Gmr, ExpectedResult>
    {
        public GmrValidationTestData()
        {
            Add(new Gmr { Id = "abcd" }, new ExpectedResult("Id", false));
            Add(new Gmr { Id = null }, new ExpectedResult("Id", true));

            Add(new Gmr { HaulierEori = "STANDARD" }, new ExpectedResult("HaulierEori", false));
            Add(new Gmr { HaulierEori = null }, new ExpectedResult("HaulierEori", true));

            Add(new Gmr { UpdatedSource = DateTime.UtcNow }, new ExpectedResult("UpdatedSource", false));
            Add(new Gmr { UpdatedSource = null }, new ExpectedResult("UpdatedSource", true));

            Add(new Gmr { Direction = "GB_TO_NI" }, new ExpectedResult("Direction", false));
            Add(new Gmr { Direction = null }, new ExpectedResult("Direction", true));

            Add(new Gmr { ActualCrossing = null }, new ExpectedResult("ActualCrossing", false));
            Add(
                new Gmr { ActualCrossing = new ActualCrossing { RouteId = "abcd" } },
                new ExpectedResult("ActualCrossing", false)
            );
            Add(
                new Gmr { ActualCrossing = new ActualCrossing { RouteId = null } },
                new ExpectedResult("ActualCrossing", true)
            );

            Add(new Gmr { CheckedInCrossing = null }, new ExpectedResult("CheckedInCrossing", false));
            Add(
                new Gmr { CheckedInCrossing = new CheckedInCrossing { RouteId = "abcd" } },
                new ExpectedResult("CheckedInCrossing", false)
            );
            Add(
                new Gmr { PlannedCrossing = new PlannedCrossing { RouteId = null } },
                new ExpectedResult("PlannedCrossing", true)
            );

            Add(new Gmr { PlannedCrossing = null }, new ExpectedResult("PlannedCrossing", false));
            Add(
                new Gmr { PlannedCrossing = new PlannedCrossing { RouteId = "abcd" } },
                new ExpectedResult("PlannedCrossing", false)
            );
            Add(
                new Gmr { PlannedCrossing = new PlannedCrossing { RouteId = null } },
                new ExpectedResult("PlannedCrossing", true)
            );
        }
    }
}
