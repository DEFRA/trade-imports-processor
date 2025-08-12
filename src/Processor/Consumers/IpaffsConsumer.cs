using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Services;
using Defra.TradeImportsProcessor.Processor.Utils;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Microsoft.Extensions.Options;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class IpaffsConsumer(IOptions<BtmsOptions> btmsOptions, IIpaffsStrategyFactory ipaffsStrategyFactory)
    : IConsumer<string>,
        IConsumerWithContext
{
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

                if (
                    ipaffsStrategyFactory.TryGetIpaffsStrategy(
                        customsDeclaration?.SubResourceType,
                        out var ipaffsStrategy
                    )
                )
                {
                    await ipaffsStrategy!.PublishToIpaffsAsync(
                        MessageId,
                        customsDeclaration?.ResourceId,
                        customsDeclaration?.Resource,
                        cancellationToken
                    );
                }
            }
        }
    }
}
