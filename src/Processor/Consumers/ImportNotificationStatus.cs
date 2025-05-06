namespace Defra.TradeImportsProcessor.Processor.Consumers;

public static class ImportNotificationStatus
{
    public const string Draft = "DRAFT";
    public const string Validated = "VALIDATED";
    public const string Rejected = "REJECTED";
    public const string PartiallyRejected = "PARTIALLY_REJECTED";
    public const string InProgress = "IN_PROGRESS";
    public const string Amend = "AMEND";
    public const string Cancelled = "CANCELLED";
    public const string Deleted = "DELETED";
}
