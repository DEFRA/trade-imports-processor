namespace Defra.TradeImportsProcessor.Testing;

public static class Endpoints
{
    public static class RawMessages
    {
        private const string Root = "/raw-messages";

        public static string Get(string messageId) => $"{Root}/{messageId}";

        public static string GetJson(string messageId) => $"{Get(messageId)}/json";
    }
}
