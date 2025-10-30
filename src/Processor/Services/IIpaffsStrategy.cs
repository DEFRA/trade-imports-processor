using Defra.TradeImportsDataApi.Domain.Events;

namespace Defra.TradeImportsProcessor.Processor.Services;

public interface IIpaffsStrategy
{
    string SupportedSubResourceType { get; }
    Task PublishToIpaffs(
        string messageId,
        string resourceId,
        CustomsDeclarationEvent customsDeclaration,
        CancellationToken cancellationToken
    );
}
