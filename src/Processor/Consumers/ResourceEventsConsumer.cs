using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Services;
using Defra.TradeImportsProcessor.Processor.Utils;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class ResourceEventsConsumer(IEnumerable<IIpaffsStrategy> strategies, ILogger<ResourceEventsConsumer> logger)
    : IConsumer<string>,
        IConsumerWithContext
{
    private string MessageId => Context.GetTransportMessage().MessageId;

    public IConsumerContext Context { get; set; } = null!;

    public async Task OnHandle(string received, CancellationToken cancellationToken)
    {
        var resourceType = Context.GetResourceType();

        if (resourceType == ResourceEventResourceTypes.CustomsDeclaration)
        {
            var message = MessageDeserializer.Deserialize<JsonElement>(received, Context.Headers.GetContentEncoding());
            var customsDeclaration = message.Deserialize<ResourceEvent<CustomsDeclarationEvent>>();

            if (string.IsNullOrEmpty(customsDeclaration?.ResourceId))
            {
                logger.LogError("Invalid resource id for {MessageId}", MessageId);
                throw new ResourceEventException(MessageId);
            }

            if (customsDeclaration.Resource is null)
            {
                logger.LogError(
                    "{MRN} Invalid resource event message received for {MessageId}",
                    customsDeclaration.ResourceId,
                    MessageId
                );
                throw new ResourceEventException(MessageId);
            }

            var strategy = strategies.FirstOrDefault(strategy =>
                strategy.SupportedSubResourceType == customsDeclaration.SubResourceType
            );

            if (strategy is not null)
            {
                await strategy.PublishToIpaffs(
                    MessageId,
                    customsDeclaration.ResourceId,
                    customsDeclaration.Resource,
                    cancellationToken
                );
            }
        }
    }
}
