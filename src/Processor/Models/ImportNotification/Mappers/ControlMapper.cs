using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ControlMapper
{
    public static IpaffsDataApi.Control Map(Control? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Control
        {
            FeedbackInformation = FeedbackInformationMapper.Map(from.FeedbackInformation),
            DetailsOnReExport = DetailsOnReExportMapper.Map(from.DetailsOnReExport),
            OfficialInspector = OfficialInspectorMapper.Map(from.OfficialInspector),
            ConsignmentLeave = from.ConsignmentLeave,
        };

        return to;
    }
}
