using Defra.TradeImportsDataApi.Domain.Gvms;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using Defra.TradeImportsProcessor.Processor.Services;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class MatchedGmrConsumer(ILogger<MatchedGmrConsumer> logger, IGmrProcessingService gmrProcessingService)
    : IConsumer<MatchedGmr>,
        IConsumerWithContext
{
    private string MessageId => Context.GetTransportMessage().MessageId;
    public required IConsumerContext Context { get; set; }

    public Task OnHandle(MatchedGmr message, CancellationToken cancellationToken)
    {
        if (message == null)
        {
            throw new GmrMessageException(MessageId);
        }
        logger.LogInformation("Received MatchedGmr for identifier {Identifier}", message.GetIdentifier);
        var dataApiGmr = (Defra.TradeImportsDataApi.Domain.Gvms.Gmr)message.Gmr;
        return gmrProcessingService.ProcessGmr(dataApiGmr, cancellationToken);
    }
}
