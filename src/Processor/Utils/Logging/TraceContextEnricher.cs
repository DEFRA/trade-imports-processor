using Serilog.Core;
using Serilog.Events;
using SlimMessageBus;
using SlimMessageBus.Host.Consumer;

namespace Defra.TradeImportsProcessor.Processor.Utils.Logging;

public class TraceContextEnricher : ILogEventEnricher
{
    private readonly string _traceHeader;
    private readonly ITraceContextAccessor _traceContextAccessor;
    public const string PropertyName = "CorrelationId";

    public TraceContextEnricher(string traceHeader)
        : this(traceHeader, new TraceContextAccessor()) { }

    private TraceContextEnricher(string traceHeader, ITraceContextAccessor traceContextAccessor)
    {
        _traceHeader = traceHeader;
        _traceContextAccessor = traceContextAccessor;
    }

    /// <inheritdoc/>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_traceContextAccessor.Context?.TraceId is null)
        {
            var consumerContext = MessageScope.Current?.GetService<IConsumerContext>();
            if (consumerContext == null)
                return;

            _traceContextAccessor.Context = new TraceContext
            {
                TraceId = consumerContext.Headers.GetTraceId(_traceHeader) ?? Guid.NewGuid().ToString("N"),
            };
        }

        logEvent.AddOrUpdateProperty(
            new LogEventProperty(PropertyName, new ScalarValue(_traceContextAccessor.Context.TraceId))
        );
    }
}
