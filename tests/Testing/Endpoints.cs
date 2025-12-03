namespace Defra.TradeImportsProcessor.Testing;

public static class Endpoints
{
    public static class RawMessages
    {
        private const string Root = "/raw-messages";

        public static string Get(string messageId) => $"{Root}/{messageId}";

        public static string GetJson(string messageId) => $"{Get(messageId)}/json";
    }

    public static class Admin
    {
        private const string Root = "/admin";

        public static class ResourceEventsDeadLetterQueue
        {
            private const string SubRoot = $"{Root}/resource-events/dlq";

            public static string Redrive() => $"{SubRoot}/redrive";

            public static string RemoveMessage(string? messageId = null) =>
                $"{SubRoot}/remove-message?messageId={messageId}";

            public static string Drain() => $"{SubRoot}/drain";
        }

        public static class CustomsDeclarationsDeadLetterQueue
        {
            private const string SubRoot = $"{Root}/customs-declarations/dlq";

            public static string Redrive() => $"{SubRoot}/redrive";

            public static string RemoveMessage(string? messageId = null) =>
                $"{SubRoot}/remove-message?messageId={messageId}";

            public static string Drain() => $"{SubRoot}/drain";
        }
    }
}
