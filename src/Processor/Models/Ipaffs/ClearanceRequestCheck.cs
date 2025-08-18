using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class ClearanceRequestCheck
{
    [JsonPropertyName("checkCode")]
    public string? CheckCode { get; set; }

    [JsonPropertyName("departmentCode")]
    public string? DepartmentCode { get; set; }
}
