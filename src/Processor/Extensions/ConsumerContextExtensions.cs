using System.Diagnostics.CodeAnalysis;
using Amazon.SQS.Model;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsDataApi.Domain.Gvms;
using Defra.TradeImportsDataApi.Domain.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Consumers;
using SlimMessageBus;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

[ExcludeFromCodeCoverage]
public static class MessageBusHeaders
{
    public const string InboundHmrcMessageTypeHeader = "InboundHmrcMessageType";
    public const string SqsBusMessage = "Sqs_Message";
    public const string ServiceBusMessage = "ServiceBus_Message";
    public const string ResourceId = "ResourceId";
}

[ExcludeFromCodeCoverage]
public static class ConsumerContextExtensions
{
    public static string GetMessageId(this IConsumerContext consumerContext)
    {
        if (consumerContext.Properties.TryGetValue(MessageBusHeaders.SqsBusMessage, out var sqsMessage))
        {
            return ((Message)sqsMessage).MessageId;
        }

        if (consumerContext.Properties.TryGetValue(MessageBusHeaders.ServiceBusMessage, out var sbMessage))
        {
            return ((ServiceBusReceivedMessage)sbMessage).MessageId;
        }

        return string.Empty;
    }

    public static string GetMessageBody(this IConsumerContext consumerContext)
    {
        if (consumerContext.Properties.TryGetValue(MessageBusHeaders.SqsBusMessage, out var sqsMessage))
        {
            return ((Message)sqsMessage).Body;
        }

        if (consumerContext.Properties.TryGetValue(MessageBusHeaders.ServiceBusMessage, out var sbMessage))
        {
            return ((ServiceBusReceivedMessage)sbMessage).Body.ToString();
        }

        return string.Empty;
    }

    public static string GetResourceType(this IConsumerContext consumerContext)
    {
        if (consumerContext.Headers.TryGetValue(MessageBusHeaders.InboundHmrcMessageTypeHeader, out var value))
        {
            return value.ToString()!;
        }

        return consumerContext.Consumer switch
        {
            GmrsConsumer => nameof(Gmr),
            NotificationConsumer => nameof(ImportPreNotification),
            _ => "Unknown",
        };
    }

    public static string GetResourceId(this IConsumerContext consumerContext)
    {
        if (consumerContext.Headers.TryGetValue(MessageBusHeaders.ResourceId, out var value))
        {
            return value.ToString()!;
        }

        return string.Empty;
    }
}
