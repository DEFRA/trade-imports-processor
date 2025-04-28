using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ControlMapper
{
    public static IpaffsDataApi.Control Map(Control? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.Control();
        to.FeedbackInformation = FeedbackInformationMapper.Map(from.FeedbackInformation);
        to.DetailsOnReExport = DetailsOnReExportMapper.Map(from.DetailsOnReExport);
        to.OfficialInspector = OfficialInspectorMapper.Map(from.OfficialInspector);
        to.ConsignmentLeave = ControlConsignmentLeaveEnumMapper.Map(from.ConsignmentLeave);
        return to;
    }
}
