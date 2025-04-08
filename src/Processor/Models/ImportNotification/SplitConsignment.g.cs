//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable

using System.Text.Json.Serialization;
using System.Dynamic;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;


/// <summary>
/// Present if the consignment has been split
/// </summary>
public partial class SplitConsignment  //
{


    /// <summary>
    /// Reference number of the valid split consignment
    /// </summary>
    [JsonPropertyName("validReferenceNumber")]
    public string? ValidReferenceNumber { get; set; }


    /// <summary>
    /// Reference number of the rejected split consignment
    /// </summary>
    [JsonPropertyName("rejectedReferenceNumber")]
    public string? RejectedReferenceNumber { get; set; }

}