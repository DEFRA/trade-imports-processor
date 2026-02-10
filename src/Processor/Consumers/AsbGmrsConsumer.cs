using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using Defra.TradeImportsProcessor.Processor.Services;
using SlimMessageBus;
using SlimMessageBus.Host.AzureServiceBus;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class AsbGmrsConsumer(ILogger<AsbGmrsConsumer> logger, IGmrProcessingService gmrProcessingService)
    : IConsumer<JsonElement>,
        IConsumerWithContext
{
    private string MessageId => Context.GetTransportMessage().MessageId;
    public required IConsumerContext Context { get; set; }

    public Task OnHandle(JsonElement message, CancellationToken cancellationToken)
    {
        var gmr = message.Deserialize<Gmr>();
        if (gmr == null)
        {
            throw new GmrMessageException(MessageId);
        }

        using (
            logger.BeginScope(
                new Dictionary<string, object>
                {
                    ["event.id"] = Context.GetMessageId(),
                    ["event.reference"] = gmr.GmrId!,
                    ["event.type"] = ResourceTypes.Gmr,
                    ["event.provider"] = nameof(AsbGmrsConsumer),
                }
            )
        )
        {
            logger.LogInformation("Received Gmr for {GmrId}", gmr.GmrId);

            var dataApiGmr = (DataApiGvms.Gmr)gmr;
            return gmrProcessingService.ProcessGmr(dataApiGmr, cancellationToken);
        }
    }
}
