using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using SlimMessageBus;
using IpaffsClearanceRequest = Defra.TradeImportsProcessor.Processor.Models.Ipaffs.ClearanceRequest;

namespace Defra.TradeImportsProcessor.Processor.Services;

public class ClearanceRequestStrategy(IMessageBus azureServiceBus, ILogger<ClearanceRequestStrategy> logger)
    : IIpaffsStrategy
{
    private const string ClearanceRequestMessageType = "ALVSClearanceRequest";
    private const string ClearanceRequestSubType = "CDS";

    public string SupportedSubResourceType => ResourceEventSubResourceTypes.ClearanceRequest;

    public async Task PublishToIpaffs(
        string messageId,
        string resourceId,
        CustomsDeclaration customsDeclaration,
        CancellationToken cancellationToken
    )
    {
        if (customsDeclaration.ClearanceRequest is null)
        {
            logger.LogError("{MRN} Invalid resource event message received for {MessageId}", resourceId, messageId);
            throw new ResourceEventException(messageId);
        }

        var clearanceRequest = new IpaffsClearanceRequest(resourceId, customsDeclaration.ClearanceRequest);

        await azureServiceBus.Publish(
            clearanceRequest,
            headers: new Dictionary<string, object>
            {
                ["messageType"] = ClearanceRequestMessageType,
                ["subType"] = ClearanceRequestSubType,
                ["PublisherType"] = "IPAFFS",
            },
            cancellationToken: cancellationToken
        );
        logger.LogInformation(
            "{MRN} {MessageType} Message successfully published to IPAFFS for {MessageId}",
            resourceId,
            ClearanceRequestMessageType,
            messageId
        );
    }
}
