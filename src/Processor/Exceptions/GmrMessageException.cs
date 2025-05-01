namespace Defra.TradeImportsProcessor.Processor.Exceptions;

public class GmrMessageException(string messageId) : Exception($"Invalid GMR message received for {messageId}");
