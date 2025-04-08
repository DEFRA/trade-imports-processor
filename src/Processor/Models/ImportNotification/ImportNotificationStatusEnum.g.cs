
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ImportNotificationStatusEnum
{

    [EnumMember(Value = "DRAFT")]
    Draft,

    [EnumMember(Value = "SUBMITTED")]
    Submitted,

    [EnumMember(Value = "VALIDATED")]
    Validated,

    [EnumMember(Value = "REJECTED")]
    Rejected,

    [EnumMember(Value = "IN_PROGRESS")]
    InProgress,

    [EnumMember(Value = "AMEND")]
    Amend,

    [EnumMember(Value = "MODIFY")]
    Modify,

    [EnumMember(Value = "REPLACED")]
    Replaced,

    [EnumMember(Value = "CANCELLED")]
    Cancelled,

    [EnumMember(Value = "DELETED")]
    Deleted,

    [EnumMember(Value = "PARTIALLY_REJECTED")]
    PartiallyRejected,

    [EnumMember(Value = "SPLIT_CONSIGNMENT")]
    SplitConsignment,

}