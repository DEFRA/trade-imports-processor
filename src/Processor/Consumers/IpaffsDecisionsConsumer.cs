using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Utils;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Microsoft.Extensions.Options;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class IpaffsDecisionsConsumer(
    IOptions<BtmsOptions> btmsOptions,
    IMessageBus azureServiceBus,
    ILogger<IpaffsDecisionsConsumer> logger
) : IConsumer<string>, IConsumerWithContext
{
    private const string DecisionNotificationMessageType = "ALVSDecisionNotification";
    private const string DecisionNotificationSubType = "ALVS";

    private string MessageId => Context.GetTransportMessage().MessageId;

    public IConsumerContext Context { get; set; } = null!;

    public async Task OnHandle(string received, CancellationToken cancellationToken)
    {
        if (btmsOptions.Value.OperatingMode == OperatingMode.Cutover)
        {
            var resourceType = Context.GetResourceType();

            if (resourceType == ResourceEventResourceTypes.CustomsDeclaration)
            {
                var message = MessageDeserializer.Deserialize<JsonElement>(
                    received,
                    Context.Headers.GetContentEncoding()
                );
                var customsDeclaration = message.Deserialize<ResourceEvent<CustomsDeclaration>>();

                if (customsDeclaration?.SubResourceType == ResourceEventSubResourceTypes.ClearanceDecision)
                {
                    if (customsDeclaration.Resource?.ClearanceDecision is null)
                    {
                        logger.LogError("Invalid resource event message received for {MessageId}", MessageId);
                        throw new ResourceEventException(MessageId);
                    }

                    var clearanceDecision = new DecisionNotification(
                        customsDeclaration.ResourceId,
                        customsDeclaration.Resource?.ClearanceDecision!
                    );

                    await azureServiceBus.Publish(
                        clearanceDecision,
                        headers: new Dictionary<string, object>
                        {
                            ["messageType"] = DecisionNotificationMessageType,
                            ["subType"] = DecisionNotificationSubType,
                        },
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
    }
}
