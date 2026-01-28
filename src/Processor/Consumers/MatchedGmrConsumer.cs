using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using Defra.TradeImportsProcessor.Processor.Services;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class MatchedGmrConsumer(ILogger<MatchedGmrConsumer> logger, IGmrProcessingService gmrProcessingService)
    : IConsumer<JsonElement>,
        IConsumerWithContext
{
    private string MessageId => Context.GetTransportMessage().MessageId;
    public required IConsumerContext Context { get; set; }

    public async Task OnHandle(JsonElement message, CancellationToken cancellationToken)
    {
        var matchedGmr = message.Deserialize<MatchedGmr>();
        if (matchedGmr == null)
        {
            throw new GmrMessageException(MessageId);
        }

        logger.LogInformation("Received MatchedGmr for identifier {Identifier}", matchedGmr.GetIdentifier);

        var dataApiGmr = (Defra.TradeImportsDataApi.Domain.Gvms.Gmr)matchedGmr.Gmr;
        await gmrProcessingService.ProcessGmr(dataApiGmr, cancellationToken);
    }
}
