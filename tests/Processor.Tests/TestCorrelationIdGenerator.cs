using Defra.TradeImportsProcessor.Processor.Utils.CorrelationId;

namespace Defra.TradeImportsProcessor.Processor.Tests
{
    internal class TestCorrelationIdGenerator(string value) : ICorrelationIdGenerator
    {
        public string Generate()
        {
            return value;
        }
    }
}
