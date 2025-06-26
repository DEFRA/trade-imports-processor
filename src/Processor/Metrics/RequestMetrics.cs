using System.Diagnostics;
using System.Diagnostics.Metrics;
using Amazon.CloudWatch.EMF.Model;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

public class RequestMetrics
{
    private readonly Counter<long> _requestsReceived;
    private readonly Counter<long> _requestsFaulted;
    private readonly Histogram<double> _requestDuration;

    public RequestMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MetricsConstants.MetricNames.MeterName);

        _requestsReceived = meter.CreateCounter<long>(
            "RequestReceived",
            nameof(Unit.COUNT),
            "Count of messages received"
        );

        _requestDuration = meter.CreateHistogram<double>(
            "RequestDuration",
            nameof(Unit.MILLISECONDS),
            "Duration of request"
        );

        _requestsFaulted = meter.CreateCounter<long>("RequestFaulted", nameof(Unit.COUNT), "Count of request faults");
    }

    public void RequestCompleted(string requestPath, string httpMethod, int statusCode, double milliseconds)
    {
        var tagList = BuildTags(requestPath, httpMethod, statusCode);

        _requestsReceived.Add(1, tagList);
        _requestDuration.Record(milliseconds, tagList);
    }

    public void RequestFaulted(string requestPath, string httpMethod, int statusCode, Exception exception)
    {
        var tagList = BuildTags(requestPath, httpMethod, statusCode);

        tagList.Add(MetricsConstants.RequestTags.ExceptionType, exception.GetType().Name);

        _requestsFaulted.Add(1, tagList);
    }

    private static TagList BuildTags(string requestPath, string httpMethod, int statusCode)
    {
        return new TagList
        {
            { MetricsConstants.RequestTags.Service, Process.GetCurrentProcess().ProcessName },
            { MetricsConstants.RequestTags.RequestPath, requestPath },
            { MetricsConstants.RequestTags.HttpMethod, httpMethod },
            { MetricsConstants.RequestTags.StatusCode, statusCode },
        };
    }
}
