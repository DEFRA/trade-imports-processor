namespace Defra.TradeImportsProcessor.Processor.Exceptions;

public class CustomsDeclarationMessageException(string messageId)
    : Exception($"Invalid customs declaration message received for {messageId}");
