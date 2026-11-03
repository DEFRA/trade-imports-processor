using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Validation.Iuu;
using FluentValidation.TestHelper;
using static Defra.TradeImportsProcessor.TestFixtures.IuuNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.Iuu;

public class IuuNotificationValidatorTests
{
    private readonly IuuNotificationValidator _validator = new();

    [Fact]
    public void WhenIuuNotificationInvalid_ThenShouldFailValidation()
    {
        var invalidIuuNotification = IuuNotificationFixture(iuuCheckOutcome: "INVALID_VALUE").Create();

        var result = _validator.TestValidate(invalidIuuNotification);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor("IuuCheckOutcome");
        var error = result.Errors.Find(e => e.PropertyName == "IuuCheckOutcome");
        Assert.NotNull(error);
        error.ErrorMessage.Should().Be($"Invalid IUU Check Outcome: INVALID_VALUE.");
    }

    [Theory]
    [InlineData("IUUOK")]
    [InlineData("IUUNA")]
    [InlineData("IUUNotCompliant")]
    public void WhenIuuNotificationIsValid_ThenShouldPassValidation(string iuuCheckOutcomeValue)
    {
        var invalidIuuNotification = IuuNotificationFixture(iuuCheckOutcome: iuuCheckOutcomeValue).Create();

        var result = _validator.TestValidate(invalidIuuNotification);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }
}
