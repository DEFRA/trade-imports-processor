using Defra.TradeImportsProcessor.Processor.Extensions;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Tests.Extensions;

public class FluentValidationExtensionsTests
{
    private class TestModel
    {
        public string? TestProperty { get; set; }
        public string? CorrelationId { get; set; }
    }

    private class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator()
        {
            RuleFor(x => x.TestProperty).NotEmpty().WithBtmsErrorCode("TEST001", x => x.CorrelationId);
        }
    }

    private readonly TestValidator _validator = new();

    [Fact]
    public void WithBtmsErrorCode_SetsTheCorrectErrorMessageAndCode()
    {
        var model = new TestModel { TestProperty = null, CorrelationId = "CORRELATION123" };

        var result = _validator.Validate(model);

        var error = result.Errors[0];
        Assert.Equal(
            "There is an issue with the Test Property. Your request with correlation Id CORRELATION123 has been terminated.",
            error.ErrorMessage
        );
        Assert.Equal("TEST001", error.CustomState);
    }

    [Fact]
    public void WithBtmsErrorCode_WithCorrelationIdAsAFunc_SetsTheCorrectErrorCode()
    {
        var model = new TestModel { TestProperty = null, CorrelationId = "CORRELATION_ID" };
        var result = _validator.Validate(model);

        var error = result.Errors[0];
        Assert.Equal(
            "There is an issue with the Test Property. Your request with correlation Id CORRELATION_ID has been terminated.",
            error.ErrorMessage
        );
        Assert.Equal("TEST001", error.CustomState);
    }

    [Fact]
    public void WithBtmsErrorCode_WithNoCorrelationId_UsesUNKNOWN()
    {
        var model = new TestModel { TestProperty = null, CorrelationId = null };

        var result = _validator.Validate(model);

        var error = result.Errors[0];
        Assert.Equal(
            "There is an issue with the Test Property. Your request with correlation Id UNKNOWN has been terminated.",
            error.ErrorMessage
        );
        Assert.Equal("TEST001", error.CustomState);
    }

    [Fact]
    public void WithBtmsErrorCode_WhenPassesValidation_ShouldDoNothing()
    {
        var model = new TestModel { TestProperty = "Bob", CorrelationId = "CORRELATION_ID" };

        var result = _validator.Validate(model);

        Assert.Empty(result.Errors);
    }
}
