using AutoFixture;
using BtmsGateway.Contract.CustomsDeclarations;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Extensions.Model;
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
            new InboundErrorItem { ErrorCode = "UNKNOWN", ErrorMessage = "Unknown error" },
            new InboundErrorItem { ErrorCode = "HMRCVAL101", ErrorMessage = "A valid error code" },
        }.ToArray();

        var inboundError = InboundErrorFixture()
            .With(i => i.Errors, inboundErrorItems)
            .Create()
            .ToDataApiExternalError();

        var result = await _errorNotificationValidator.TestValidateAsync(inboundError);

        result.ShouldHaveValidationErrorFor(e => e.Errors);
    }

    [Fact]
    public async Task Validate_DoesNotReturnAnError_WhenAllTheErrorCodesAreValid()
    {
        var inboundErrorItems = new List<InboundErrorItem>
        {
            new InboundErrorItem { ErrorCode = "HMRCVAL101", ErrorMessage = "A valid error code" },
            new InboundErrorItem { ErrorCode = "HMRCVAL102", ErrorMessage = "A valid error code" },
        }.ToArray();

        var inboundError = InboundErrorFixture()
            .With(i => i.Errors, inboundErrorItems)
            .Create()
            .ToDataApiExternalError();

        var result = await _errorNotificationValidator.TestValidateAsync(inboundError);

        result.ShouldNotHaveValidationErrorFor(e => e.Errors);
    }
}
