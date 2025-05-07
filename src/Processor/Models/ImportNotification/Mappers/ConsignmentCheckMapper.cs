using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ConsignmentCheckMapper
{
    public static IpaffsDataApi.ConsignmentCheck Map(ConsignmentCheck? from)
    {
        if (from is null)
        {
            return null!;
        }

        return new IpaffsDataApi.ConsignmentCheck
        {
            EuStandard = from.EuStandard,
            AdditionalGuarantees = from.AdditionalGuarantees,
            DocumentCheckResult = from.DocumentCheckResult,
            NationalRequirements = from.NationalRequirements,
            IdentityCheckDone = from.IdentityCheckDone,
            IdentityCheckType = from.IdentityCheckType,
            IdentityCheckResult = from.IdentityCheckResult,
            IdentityCheckNotDoneReason = from.IdentityCheckNotDoneReason,
            PhysicalCheckDone = from.PhysicalCheckDone,
            PhysicalCheckResult = from.PhysicalCheckResult,
            PhysicalCheckNotDoneReason = from.PhysicalCheckNotDoneReason,
            PhysicalCheckOtherText = from.PhysicalCheckOtherText,
            WelfareCheck = from.WelfareCheck,
            NumberOfAnimalsChecked = from.NumberOfAnimalsChecked,
            LaboratoryCheckDone = from.LaboratoryCheckDone,
            LaboratoryCheckResult = from.LaboratoryCheckResult,
            DocumentCheckAdditionalDetails = from.DocumentCheckAdditionalDetails,
        };
    }
}
