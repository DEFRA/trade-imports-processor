using AutoFixture;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.TestHelper;
using static Defra.TradeImportsProcessor.TestFixtures.InboundErrorFixtures;
using DataApiErrors = Defra.TradeImportsDataApi.Domain.Errors;

namespace Defra.TradeImportsProcessor.Processor.Tests.Validation.CustomsDeclarations;

public class ErrorNotificationValidatorTests
{
    private readonly ErrorNotificationValidator _errorNotificationValidator = new();

    [Fact]
    public async Task Validate_ReturnsErrors_IfAnyOfTheErrorCodesAreInvalid()
    {
        var inboundErrorItems = new List<InboundErrorItem>
        {
            new InboundErrorItem { errorCode = "UNKNOWN", errorMessage = "Unknown error" },
            new InboundErrorItem { errorCode = "HMRCVAL101", errorMessage = "A valid error code" },
        }.ToArray();

        var inboundError = (ExternalError)InboundErrorFixture().With(i => i.Errors, inboundErrorItems).Create();

        var result = await _errorNotificationValidator.TestValidateAsync(inboundError);

        result.ShouldHaveValidationErrorFor(e => e.Errors);
    }

    [Fact]
    public async Task Validate_DoesNotReturnAnError_WhenAllTheErrorCodesAreValid()
    {
        var inboundErrorItems = new List<InboundErrorItem>
        {
            new InboundErrorItem { errorCode = "HMRCVAL101", errorMessage = "A valid error code" },
            new InboundErrorItem { errorCode = "HMRCVAL102", errorMessage = "A valid error code" },
        }.ToArray();

        var inboundError = (ExternalError)InboundErrorFixture().With(i => i.Errors, inboundErrorItems).Create();

        var result = await _errorNotificationValidator.TestValidateAsync(inboundError);

        result.ShouldNotHaveValidationErrorFor(e => e.Errors);
    }
}
