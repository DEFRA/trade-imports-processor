using System.Diagnostics.CodeAnalysis;
using Amazon.SQS.Model;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using SlimMessageBus;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

[ExcludeFromCodeCoverage]
public static class MessageBusHeaders
{
    public const string InboundHmrcMessageTypeHeader = "InboundHmrcMessageType";
    public const string ResourceTypeHeader = "ResourceType";
    public const string SubResourceTypeHeader = "SubResourceType";
    public const string SqsBusMessage = "Sqs_Message";
    public const string ServiceBusMessage = "ServiceBus_Message";
    public const string ResourceId = "ResourceId";
    public const string TraceId = "x-cdp-request-id";
    public const string ContentEncoding = "Content-Encoding";
}

[ExcludeFromCodeCoverage]
public static class ResourceTypes
{
    public const string Unknown = "Unknown";
    public const string Gmr = nameof(TradeImportsDataApi.Domain.Gvms.Gmr);
    public const string ImportPreNotification = nameof(TradeImportsDataApi.Domain.Ipaffs.ImportPreNotification);
    public const string ClearanceRequest = InboundHmrcMessageType.ClearanceRequest;
    public const string InboundError = InboundHmrcMessageType.InboundError;
    public const string Finalisation = InboundHmrcMessageType.Finalisation;
    public const string CustomsDeclaration = nameof(TradeImportsDataApi.Domain.CustomsDeclaration);
    public const string ProcessingError = nameof(TradeImportsDataApi.Domain.Errors.ProcessingError);
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

    public static string GetResourceType(this IConsumerContext consumerContext)
    {
        if (consumerContext.Headers.TryGetValue(MessageBusHeaders.InboundHmrcMessageTypeHeader, out var value))
        {
            return value.ToString()! switch
            {
                ResourceTypes.ClearanceRequest => ResourceTypes.ClearanceRequest,
                ResourceTypes.Finalisation => ResourceTypes.Finalisation,
                ResourceTypes.InboundError => ResourceTypes.InboundError,
                _ => ResourceTypes.Unknown,
            };
        }

        if (consumerContext.Headers.TryGetValue(MessageBusHeaders.ResourceTypeHeader, out var resourceTypeValue))
        {
            return resourceTypeValue.ToString()! switch
            {
                ResourceTypes.CustomsDeclaration => ResourceTypes.CustomsDeclaration,
                ResourceTypes.ImportPreNotification => ResourceTypes.ImportPreNotification,
                ResourceTypes.ProcessingError => ResourceTypes.ProcessingError,
                _ => ResourceTypes.Unknown,
            };
        }

        return consumerContext.Consumer switch
        {
            AsbGmrsConsumer => ResourceTypes.Gmr,
            NotificationConsumer => ResourceTypes.ImportPreNotification,
            _ => ResourceTypes.Unknown,
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
