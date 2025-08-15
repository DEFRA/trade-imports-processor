using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Amazon.CloudWatch.EMF.Model;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class ConsumerMetrics : IConsumerMetrics
{
    private readonly Histogram<double> _consumeDuration;
    private readonly Counter<long> _consumeTotal;
    private readonly Counter<long> _consumeFaultTotal;
    private readonly Counter<long> _consumeWarnTotal;
    private readonly Counter<long> _consumeInProgress;

    public ConsumerMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MetricsConstants.MetricNames.MeterName);

        _consumeDuration = meter.CreateHistogram<double>(
            "MessagingConsumeDuration",
            nameof(Unit.MILLISECONDS),
            "Elapsed time spent consuming a message, in millis"
        );
        _consumeTotal = meter.CreateCounter<long>(
            "MessagingConsume",
            nameof(Unit.COUNT),
            description: "Number of messages consumed"
        );
        _consumeFaultTotal = meter.CreateCounter<long>(
            "MessagingConsumeErrors",
            nameof(Unit.COUNT),
            description: "Number of message consume faults"
        );
        _consumeWarnTotal = meter.CreateCounter<long>(
            "MessagingConsumeWarnings",
            nameof(Unit.COUNT),
            description: "Number of message consume warnings"
        );
        _consumeInProgress = meter.CreateCounter<long>(
            "MessagingConsumeActive",
            nameof(Unit.COUNT),
            description: "Number of consumptions in progress"
        );
    }

    public void Start(string queueName, string consumerName, string resourceType)
    {
        var tagList = BuildTags(queueName, consumerName, resourceType);

        _consumeTotal.Add(1, tagList);
        _consumeInProgress.Add(1, tagList);
    }

    public void Faulted(string queueName, string consumerName, string resourceType, Exception exception)
    {
        var tagList = BuildTags(queueName, consumerName, resourceType);

        tagList.Add(Constants.Tags.ExceptionType, exception.GetType().Name);

        _consumeFaultTotal.Add(1, tagList);
    }

    public void Warn(string queueName, string consumerName, string resourceType, Exception exception)
    {
        var tagList = BuildTags(queueName, consumerName, resourceType);

        tagList.Add(Constants.Tags.ExceptionType, exception.GetType().Name);

        _consumeWarnTotal.Add(1, tagList);
    }

    public void Complete(string queueName, string consumerName, double milliseconds, string resourceType)
    {
        var tagList = BuildTags(queueName, consumerName, resourceType);

        _consumeInProgress.Add(-1, tagList);
        _consumeDuration.Record(milliseconds, tagList);
    }

    private static TagList BuildTags(string queueName, string consumerName, string resourceType)
    {
        return new TagList
        {
            { Constants.Tags.Service, Process.GetCurrentProcess().ProcessName },
            { Constants.Tags.QueueName, queueName },
            { Constants.Tags.ConsumerType, consumerName },
            { Constants.Tags.ResourceType, resourceType },
        };
    }

    private static class Constants
    {
        public static class Tags
        {
            public const string QueueName = "QueueName";
            public const string ConsumerType = "ConsumerType";
            public const string Service = "ServiceName";
            public const string ExceptionType = "ExceptionType";
            public const string ResourceType = "ResourceType";
        }
    }
}
