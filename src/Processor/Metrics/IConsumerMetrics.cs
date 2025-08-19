namespace Defra.TradeImportsProcessor.Processor.Metrics;

public interface IConsumerMetrics
{
    void Start(string queueName, string consumerName, string resourceType);
    void Faulted(string queueName, string consumerName, string resourceType, Exception exception);
    void Warn(string queueName, string consumerName, string resourceType, Exception exception);
    void Complete(string queueName, string consumerName, double milliseconds, string resourceType);
}
