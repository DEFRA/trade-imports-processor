using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class CommodityValidator : AbstractValidator<Commodity>
{
    public CommodityValidator(string correlationId)
    {
        RuleFor(p => p.ItemNumber).NotNull().InclusiveBetween(1, 999);
        RuleFor(p => p.CustomsProcedureCode).NotEmpty().MaximumLength(7);
        RuleFor(p => p.TaricCommodityCode).NotEmpty().Matches("\\d").Length(10);
        RuleFor(p => p.GoodsDescription).NotEmpty().MaximumLength(280);

        RuleFor(p => p.ConsigneeId).NotEmpty().MaximumLength(18);
        RuleFor(p => p.ConsigneeName).NotEmpty().MaximumLength(35);
        RuleFor(p => p.NetMass).NotNull();
        RuleFor(p => p.OriginCountryCode).NotNull().Length(2);

        RuleForEach(p => p.Documents)
            .SetValidator(item => new ImportDocumentValidator((int)item.ItemNumber!, correlationId));
        RuleForEach(p => p.Checks).SetValidator(item => new CheckValidator((int)item.ItemNumber!, correlationId));

        // CDMS-275 - NEW
        RuleFor(p => p.SupplementaryUnits)
            .Must(HaveAValidCommodityDecimalFormat)
            .WithState(_ => "ALVSVAL108")
            .WithMessage(p =>
                $"Supplementary units format on item number {p.ItemNumber} is invalid. Your request with correlation ID {correlationId} has been terminated. Enter it in the format 99999999999.999."
            );

        // CDMS-254 - NEW
        RuleFor(p => p.NetMass)
            .Must(HaveAValidCommodityDecimalFormat)
            .WithState(_ => "ALVSVAL109")
            .WithMessage(p =>
                $"Net mass format on item number {p.ItemNumber} is invalid. Your request with correlation ID {correlationId} has been terminated. Enter it in the format 99999999999.999."
            );

        // CDMS-249 / CDMS-699 - NEW
        RuleFor(p => p.Documents)
            .NotEmpty()
            .WithState(_ => "ALVSVAL318")
            .WithMessage(p =>
                $"Item {p.ItemNumber} has no document code. BTMS requires at least one item document. Your request with correlation ID {correlationId} has been terminated."
            )
            .When(IsNotAGmsNotification);

        // CDMS-265
        RuleForEach(p => p.Documents)
            .Must(MustHaveCorrectDocumentCodesForChecks)
            .WithMessage(
                (item, document) =>
                    $"Document code {document.DocumentCode} is not appropriate for the check code requested on ItemNumber {item.ItemNumber}. Your request with correlation ID {correlationId} has been terminated."
            )
            .WithState(_ => "ALVSVAL320")
            .When(p => p.Checks is not null);

        // CDMS-328 / CDMS-700
        RuleForEach(p => p.Checks)
            .Where(IsNotAGmsCheckCode)
            .Must(MustHaveDocumentForCheck)
            .WithMessage(
                (item, check) =>
                    $"Check code {check.CheckCode} on ItemNumber {item.ItemNumber} must have a document code. Your request with Correlation ID {correlationId} has been terminated."
            )
            .WithState(_ => "ALVSVAL321");

        // CDMS-327 / CDMS-698
        RuleFor(p => p.Checks)
            .Must(MustOnlyHaveOneCheckPerAuthority!)
            .WithMessage(p =>
                $"Item {p.ItemNumber} has more than one Item Check defined for the same authority. You can only provide one. Your service request with Correlation ID {correlationId} has been terminated."
            )
            .WithState(_ => "ALVSVAL317")
            .When(p => p.Checks is not null);

        // CDMS-267
        RuleFor(p => p.Checks)
            .Must(MustHavePoAoCheck!)
            .WithMessage(p =>
                $"An IUU document has been specified for ItemNumber {p.ItemNumber}. Request a manual clearance if the item does not require a CHED P. Your request with correlation ID {correlationId} has been terminated."
            )
            .WithState(_ => "ALVSVAL328")
            .When(x => x.Checks is not null && x.Checks.Any(y => y.CheckCode == "H224"));
    }

    private static bool IsNotAGmsCheckCode(CommodityCheck check)
    {
        return check.CheckCode != "H220";
    }

    private static bool IsNotAGmsNotification(Commodity commodity)
    {
        return commodity.Checks == null || commodity.Checks.All(IsNotAGmsCheckCode);
    }

    private static bool MustHaveCorrectDocumentCodesForChecks(Commodity commodity, ImportDocument importDocument)
    {
        var checkCodes = AuthorityCodeMappings
            .Where(x => x.DocumentCode == importDocument.DocumentCode)
            .Select(x => x.CheckCode);
        return commodity.Checks != null && commodity.Checks.Any(x => checkCodes.Contains(x.CheckCode));
    }

    private static bool MustHaveDocumentForCheck(Commodity commodity, CommodityCheck check)
    {
        var documentCodes = AuthorityCodeMappings
            .Where(x => x.CheckCode == check.CheckCode)
            .Select(x => x.DocumentCode);
        return commodity.Documents != null && commodity.Documents.Any(x => documentCodes.Contains(x.DocumentCode));
    }

    private static bool HaveAValidCommodityDecimalFormat(Commodity commodity, decimal? value)
    {
        var supplementaryUnits = value.ToString() ?? "";
        var length = supplementaryUnits.Replace(".", "").Length;
        var numDecimals = supplementaryUnits.SkipWhile(c => c != '.').Skip(1).Count();
        return length <= 14 && numDecimals <= 3;
    }

    private static bool IsNotAnIuuCheckCode(string? checkCode)
    {
        return checkCode != "H224";
    }

    private static bool MustOnlyHaveOneCheckPerAuthority(Commodity commodity, CommodityCheck[] checks)
    {
        var checkCodes = checks.Select(x => x.CheckCode).Where(IsNotAnIuuCheckCode);

        var authorityCheckCodeMatches = AuthorityCodeMappings
            .DistinctBy(a => a.CheckCode)
            .Where(a => checkCodes.Contains(a.CheckCode))
            .GroupBy(a => a.Name);

        return authorityCheckCodeMatches.All(a => a.Count() <= 1);
    }

    private static bool MustHavePoAoCheck(Commodity commodity, CommodityCheck[] checks)
    {
        return checks.Any(x => x.CheckCode == "H222");
    }

    public sealed record AuthorityCodeMap(string Name, string DocumentCode, string CheckCode);

    public static readonly List<AuthorityCodeMap> AuthorityCodeMappings =
    [
        new("hmi", "N002", "H218"),
        new("hmi", "N002", "H220"),
        new("hmi", "C085", "H218"),
        new("hmi", "C085", "H220"),
        new("hmi", "9HCG", "H220"),
        new("phsi", "N851", "H219"),
        new("phsi", "9115", "H219"),
        new("phsi", "C085", "H219"),
        new("pha", "C673", "H224"),
        new("pha", "C641", "H224"),
        new("pha", "N853", "H222"),
        new("pha", "C678", "H223"),
        new("apha", "C640", "H221"),
    ];
}
