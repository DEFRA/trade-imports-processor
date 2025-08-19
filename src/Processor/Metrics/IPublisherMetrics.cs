namespace Defra.TradeImportsProcessor.Processor.Metrics;

public interface IPublisherMetrics
{
    void Start(string queueName, string publisherType, string resourceType);
    void Faulted(string queueName, string publisherType, string resourceType, Exception exception);
    void Complete(string queueName, string publisherType, string resourceType, double milliseconds);
}
