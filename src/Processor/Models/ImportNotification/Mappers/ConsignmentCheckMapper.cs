using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ConsignmentCheckMapper
{
    public static IpaffsDataApi.ConsignmentCheck Map(ConsignmentCheck? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.ConsignmentCheck();
        to.EuStandard = from.EuStandard;
        to.AdditionalGuarantees = from.AdditionalGuarantees;
        to.DocumentCheckResult = from.DocumentCheckResult;
        to.NationalRequirements = from.NationalRequirements;
        to.IdentityCheckDone = from.IdentityCheckDone;
        to.IdentityCheckType = ConsignmentCheckIdentityCheckTypeEnumMapper.Map(from.IdentityCheckType);
        to.IdentityCheckResult = from.IdentityCheckResult;
        to.IdentityCheckNotDoneReason = ConsignmentCheckIdentityCheckNotDoneReasonEnumMapper.Map(
            from.IdentityCheckNotDoneReason
        );
        to.PhysicalCheckDone = from.PhysicalCheckDone;
        to.PhysicalCheckResult = from.PhysicalCheckResult;
        to.PhysicalCheckNotDoneReason = ConsignmentCheckPhysicalCheckNotDoneReasonEnumMapper.Map(
            from.PhysicalCheckNotDoneReason
        );
        to.PhysicalCheckOtherText = from.PhysicalCheckOtherText;
        to.WelfareCheck = from.WelfareCheck;
        to.NumberOfAnimalsChecked = from.NumberOfAnimalsChecked;
        to.LaboratoryCheckDone = from.LaboratoryCheckDone;
        to.LaboratoryCheckResult = from.LaboratoryCheckResult;
        return to;
    }
}
