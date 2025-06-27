using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Amazon.CloudWatch.EMF.Model;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class AzureMetrics
{
    private readonly Gauge<long> _deadLetterTotal;

    public AzureMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MetricsConstants.MetricNames.MeterName);

        _deadLetterTotal = meter.CreateGauge<long>(
            "MessagingAzureDeadLetter",
            nameof(Unit.COUNT),
            description: "Number of messages on DLQ"
        );
    }

    public void DeadLetter(string model, long count)
    {
        _deadLetterTotal.Record(count, BuildTags(model));
    }

    private static TagList BuildTags(string model)
    {
        return new TagList
        {
            { Constants.Tags.Service, Process.GetCurrentProcess().ProcessName },
            { Constants.Tags.Model, model },
        };
    }

    private static class Constants
    {
        public static class Tags
        {
            public const string Model = "ModelType";
            public const string Service = "ServiceName";
        }
    }
}
