using Defra.TradeImportsProcessor.Processor.Models.Iuu;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.Iuu;

public class IuuNotificationValidator : AbstractValidator<IuuNotification>
{
    private static readonly string[] s_validIuuCheckOutcomes = ["IUUOK", "IUUNA", "IUUNotCompliant"];

    public IuuNotificationValidator()
    {
        // We don't know the shape of the message yet. These validation rules could change!

        RuleFor(iuuNotification => iuuNotification.IuuCheckOutcome)
            .Must(HaveValidCheckOutcomeValue)
            .WithMessage(iuuNotification => $"Invalid IUU Check Outcome: {iuuNotification.IuuCheckOutcome}.");
    }

    private static bool HaveValidCheckOutcomeValue(string iuuCheckOutcome)
    {
        return s_validIuuCheckOutcomes.Contains(iuuCheckOutcome);
    }
}
