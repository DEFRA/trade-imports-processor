using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PurposeForImportOrAdmissionEnumMapper
{
    public static IpaffsDataApi.PurposeForImportOrAdmission? Map(PurposeForImportOrAdmission? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            PurposeForImportOrAdmission.DefinitiveImport => IpaffsDataApi.PurposeForImportOrAdmission.DefinitiveImport,
            PurposeForImportOrAdmission.HorsesReEntry => IpaffsDataApi.PurposeForImportOrAdmission.HorsesReEntry,
            PurposeForImportOrAdmission.TemporaryAdmissionHorses => IpaffsDataApi
                .PurposeForImportOrAdmission
                .TemporaryAdmissionHorses,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
