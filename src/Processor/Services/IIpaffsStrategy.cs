using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Services;

public interface IIpaffsStrategy
{
    string SupportedSubResourceType { get; }
    Task PublishToIpaffs(
        string messageId,
        string resourceId,
        CustomsDeclaration customsDeclaration,
        CancellationToken cancellationToken
    );
}
