using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Utils;
using Defra.TradeImportsProcessor.Processor.Utils.Converter;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Microsoft.Extensions.Options;
using SlimMessageBus;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class IpaffsDecisionsConsumer(
    IOptions<BtmsOptions> btmsOptions,
    IOptions<CdsOptions> cdsOptions,
    IMessageBus azureServiceBus,
    ILogger<IpaffsDecisionsConsumer> logger
) : IConsumer<string>, IConsumerWithContext
{
    private const string DecisionNotificationMessageXPath = "DecisionNotification/DecisionNotification";
    private const string DecisionNotificationMessageType = "ALVSDecisionNotification";
    private const string DecisionNotificationSubType = "ALVS";

    public IConsumerContext Context { get; set; } = null!;

    public async Task OnHandle(string received, CancellationToken cancellationToken)
    {
        if (btmsOptions.Value.OperatingMode == OperatingMode.Cutover) // Implement Operating Mode Strategy?
        {
            var resourceType = Context.GetResourceType();

            if (resourceType == ResourceEventResourceTypes.CustomsDeclaration)
            {
                var message = MessageDeserializer.Deserialize<JsonElement>(
                    received,
                    Context.Headers.GetContentEncoding()
                );
                var customsDeclaration = message.Deserialize<ResourceEvent<CustomsDeclaration>>();

                if (customsDeclaration?.Resource?.ClearanceDecision is null)
                {
                    logger.LogWarning("Invalid customs declaration decision received from queue.");
                    return;
                }

                if (customsDeclaration.SubResourceType == ResourceEventSubResourceTypes.ClearanceDecision)
                {
                    // Convert the JSON message to SOAP XML
                    var decisionNotificationSoap = ClearanceDecisionToSoapConverter.Convert(
                        customsDeclaration.Resource.ClearanceDecision,
                        customsDeclaration.ResourceId,
                        cdsOptions.Value.Username,
                        cdsOptions.Value.Password
                    );
                    var soapDocument = SoapUtils.ToXmlDocument(decisionNotificationSoap);

                    // Convert the SOAP XML to IPAFFS JSON
                    var decisionNotificationJson = SoapToJsonConverter.Convert(
                        soapDocument,
                        DecisionNotificationMessageXPath
                    );

                    if (decisionNotificationJson is null)
                    {
                        logger.LogWarning("Could not convert Decision Notification SOAP message to JSON."); // Log error?
                        return;
                    }

                    var ipaffsDecisionNotification = JsonSerializer.Deserialize<IpaffsDecisionNotification>(
                        decisionNotificationJson
                    );

                    // Send the IPAFFS Decision Notification to Azure Service Bus Topic
                    await azureServiceBus.Publish(
                        ipaffsDecisionNotification,
                        headers: new Dictionary<string, object>
                        {
                            ["messageType"] = DecisionNotificationMessageType,
                            ["subType"] =
                                SoapUtils.GetProperty(soapDocument, "SourceSystem") ?? DecisionNotificationSubType,
                        },
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
    }
}
