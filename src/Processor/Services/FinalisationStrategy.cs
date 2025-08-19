using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using SlimMessageBus;
using IpaffsFinalisation = Defra.TradeImportsProcessor.Processor.Models.Ipaffs.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.Services;

public class FinalisationStrategy(IMessageBus azureServiceBus, ILogger<FinalisationStrategy> logger) : IIpaffsStrategy
{
    private const string FinalisationMessageType = "FinalisationNotificationRequest";
    private const string FinalisationSubType = "CDS";

    public string SupportedSubResourceType => ResourceEventSubResourceTypes.Finalisation;

    public async Task PublishToIpaffs(
        string messageId,
        string resourceId,
        CustomsDeclaration customsDeclaration,
        CancellationToken cancellationToken
    )
    {
        if (customsDeclaration.Finalisation is null)
        {
            logger.LogError("{MRN} Invalid resource event message received for {MessageId}", resourceId, messageId);
            throw new ResourceEventException(messageId);
        }

        var finalisation = new IpaffsFinalisation(resourceId, customsDeclaration.Finalisation);

        await azureServiceBus.Publish(
            finalisation,
            headers: new Dictionary<string, object>
            {
                ["messageType"] = FinalisationMessageType,
                ["subType"] = FinalisationSubType,
            },
            cancellationToken: cancellationToken
        );
        logger.LogInformation(
            "{MRN} {MessageType} Message successfully published to IPAFFS for {MessageId}",
            resourceId,
            FinalisationMessageType,
            messageId
        );
    }
}
