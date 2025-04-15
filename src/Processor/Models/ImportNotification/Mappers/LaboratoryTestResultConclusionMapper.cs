using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class LaboratoryTestResultConclusionEnumMapper
{
    public static IpaffsDataApi.LaboratoryTestResultConclusion? Map(LaboratoryTestResultConclusion? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            LaboratoryTestResultConclusion.Satisfactory => IpaffsDataApi.LaboratoryTestResultConclusion.Satisfactory,
            LaboratoryTestResultConclusion.NotSatisfactory => IpaffsDataApi
                .LaboratoryTestResultConclusion
                .NotSatisfactory,
            LaboratoryTestResultConclusion.NotInterpretable => IpaffsDataApi
                .LaboratoryTestResultConclusion
                .NotInterpretable,
            LaboratoryTestResultConclusion.Pending => IpaffsDataApi.LaboratoryTestResultConclusion.Pending,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
