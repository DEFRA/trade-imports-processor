using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using SlimMessageBus;

namespace Defra.TradeImportsProcessor.Processor.Services;

public class DecisionNotificationStrategy(IMessageBus azureServiceBus, ILogger<DecisionNotificationStrategy> logger)
    : IIpaffsStrategy
{
    private const string DecisionNotificationMessageType = "ALVSDecisionNotification";
    private const string DecisionNotificationSubType = "ALVS";

    public string SupportedSubResourceType => ResourceEventSubResourceTypes.ClearanceDecision;

    public async Task PublishToIpaffs(
        string messageId,
        string resourceId,
        CustomsDeclaration customsDeclaration,
        CancellationToken cancellationToken
    )
    {
        if (customsDeclaration.ClearanceDecision is null)
        {
            logger.LogError("{MRN} Invalid resource event message received for {MessageId}", resourceId, messageId);
            throw new ResourceEventException(messageId);
        }

        var decisionNotification = new DecisionNotification(resourceId, customsDeclaration.ClearanceDecision);

        await azureServiceBus.Publish(
            decisionNotification,
            headers: new Dictionary<string, object>
            {
                ["messageType"] = DecisionNotificationMessageType,
                ["subType"] = DecisionNotificationSubType,
            },
            cancellationToken: cancellationToken
        );
        logger.LogInformation("{MRN} Message successfully published to IPAFFS for {MessageId}", resourceId, messageId);
    }
}
