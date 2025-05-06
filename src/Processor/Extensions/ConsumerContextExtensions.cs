using Defra.TradeImportsDataApi.Domain.Ipaffs;
using SlimMessageBus;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class MessageBusHeaders
{
    public const string InboundHmrcMessageTypeHeader = "InboundHmrcMessageType";
    public const string SqsBusMessage = "Sqs_Message";
}

public static class ConsumerContextExtensions
{
    public static string GetResourceType(this IConsumerContext consumerContext)
    {
        if (consumerContext.Headers.TryGetValue(MessageBusHeaders.InboundHmrcMessageTypeHeader, out var value))
        {
            return value.ToString()!;
        }

        return nameof(ImportPreNotification);
    }
}
