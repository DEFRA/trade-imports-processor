using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class DataApiCustomsDeclarationFinalisationExtensions
{
    public static FinalStateValues FinalStateValue(this DataApiCustomsDeclaration.Finalisation finalisation) =>
        Enum.Parse<FinalStateValues>(finalisation.FinalState);
}
