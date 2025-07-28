using System.Text;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class DataApiCustomsDeclarationExtensions
{
    public static string? State(
        this TradeImportsDataApi.Domain.CustomsDeclaration.CustomsDeclaration customsDeclaration
    )
    {
        var result = new StringBuilder();

        if (customsDeclaration.ClearanceRequest is not null)
            result.Append(
                $"{nameof(customsDeclaration.ClearanceRequest)}["
                    + $"{nameof(customsDeclaration.ClearanceRequest.MessageSentAt)}={customsDeclaration.ClearanceRequest.MessageSentAt:O}, "
                    + $"{nameof(customsDeclaration.ClearanceRequest.ExternalVersion)}={customsDeclaration.ClearanceRequest.ExternalVersion}"
                    + $"]"
            );

        if (customsDeclaration.ClearanceDecision is not null)
        {
            if (result.Length > 0)
                result.Append(", ");

            result.Append(
                $"{nameof(customsDeclaration.ClearanceDecision)}["
                    + $"{nameof(customsDeclaration.ClearanceDecision.DecisionNumber)}={customsDeclaration.ClearanceDecision.DecisionNumber}"
                    + $"]"
            );
        }

        if (customsDeclaration.Finalisation is not null)
        {
            if (result.Length > 0)
                result.Append(", ");

            result.Append(
                $"{nameof(customsDeclaration.Finalisation)}["
                    + $"{nameof(customsDeclaration.Finalisation.MessageSentAt)}={customsDeclaration.Finalisation.MessageSentAt:O}, "
                    + $"{nameof(customsDeclaration.Finalisation.ExternalVersion)}={customsDeclaration.Finalisation.ExternalVersion}"
                    + $"]"
            );
        }

        return result.Length == 0 ? null : result.ToString();
    }
}
