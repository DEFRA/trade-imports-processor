using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Amazon.CloudWatch.EMF.Model;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class PublisherMetrics : IPublisherMetrics
{
    private readonly Histogram<double> _publishDuration;
    private readonly Counter<long> _publishTotal;
    private readonly Counter<long> _publishFaultTotal;
    private readonly Counter<long> _publishInProgress;

    public PublisherMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MetricsConstants.MetricNames.MeterName);

        _publishDuration = meter.CreateHistogram<double>(
            "MessagingPublishDuration",
            nameof(Unit.MILLISECONDS),
            "Elapsed time spent publishing a message, in millis"
        );
        _publishTotal = meter.CreateCounter<long>(
            "MessagingPublish",
            nameof(Unit.COUNT),
            description: "Number of messages published"
        );
        _publishFaultTotal = meter.CreateCounter<long>(
            "MessagingPublishErrors",
            nameof(Unit.COUNT),
            description: "Number of message publish faults"
        );
        _publishInProgress = meter.CreateCounter<long>(
            "MessagingPublishActive",
            nameof(Unit.COUNT),
            description: "Number of publish operations in progress"
        );
    }

    public void Start(string queueName, string publisherType, string resourceType)
    {
        var tagList = BuildTags(queueName, publisherType, resourceType);

        _publishTotal.Add(1, tagList);
        _publishInProgress.Add(1, tagList);
    }

    public void Faulted(string queueName, string publisherType, string resourceType, Exception exception)
    {
        var tagList = BuildTags(queueName, publisherType, resourceType);

        tagList.Add(Constants.Tags.ExceptionType, exception.GetType().Name);

        _publishFaultTotal.Add(1, tagList);
    }

    public void Complete(string queueName, string publisherType, string resourceType, double milliseconds)
    {
        var tagList = BuildTags(queueName, publisherType, resourceType);

        _publishInProgress.Add(-1, tagList);
        _publishDuration.Record(milliseconds, tagList);
    }

    private static TagList BuildTags(string queueName, string publisherType, string resourceType)
    {
        return new TagList
        {
            { Constants.Tags.Service, Process.GetCurrentProcess().ProcessName },
            { Constants.Tags.QueueName, queueName },
            { Constants.Tags.PublisherType, publisherType },
            { Constants.Tags.ResourceType, resourceType },
        };
    }

    private static class Constants
    {
        public static class Tags
        {
            public const string QueueName = "QueueName";
            public const string PublisherType = "PublisherType";
            public const string ResourceType = "ResourceType";
            public const string Service = "ServiceName";
            public const string ExceptionType = "ExceptionType";
        }
    }
}
