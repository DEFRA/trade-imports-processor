using System.Collections.Frozen;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using FluentValidation;
using ClearanceRequest = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.ClearanceRequest;
using Finalisation = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class ClearanceRequestValidator : AbstractValidator<ClearanceRequestValidatorInput>
{
    public readonly List<CommodityDocumentCheckMap> CommodityDocumentCheckMap =
    [
        new() { CheckCode = "H221", DocumentCode = "C640" },
        new() { CheckCode = "H223", DocumentCode = "C678" },
        new() { CheckCode = "H222", DocumentCode = "N853" },
        new() { CheckCode = "H224", DocumentCode = "C673" },
        new() { CheckCode = "H219", DocumentCode = "N851" },
        new() { CheckCode = "H219", DocumentCode = "9115" },
        new() { CheckCode = "H219", DocumentCode = "C085" },
        new() { CheckCode = "H218", DocumentCode = "N002" },
        new() { CheckCode = "H218", DocumentCode = "C085" },
        new() { CheckCode = "H218", DocumentCode = "9HCG" },
        new() { CheckCode = "H220", DocumentCode = "N002" },
        new() { CheckCode = "H220", DocumentCode = "9HCG" },
    ];

    private static IEnumerable<T> OrEmpty<T>(IEnumerable<T>? items)
    {
        return items ?? [];
    }

    private static IEnumerable<ClearanceRequestValidatorInputSubject<T>> WithSubject<T>(
        ClearanceRequestValidatorInput input,
        IEnumerable<T> subjects
    )
    {
        return subjects.Select(subject => new ClearanceRequestValidatorInputSubject<T>
        {
            NewClearanceRequest = input.NewClearanceRequest,
            ExistingClearanceRequest = input.ExistingClearanceRequest,
            ExistingFinalisation = input.ExistingFinalisation,
            Mrn = input.Mrn,
            Subject = subject,
        });
    }

    public ClearanceRequestValidator()
    {
        var validDocumentCodes = CommodityDocumentCheckMap.Select(c => c.DocumentCode).Distinct().ToList();

        RuleForEach(p => p.NewClearanceRequest.Commodities)
            .Must(commodity => HaveAValidCommodityDecimalFormat(commodity.SupplementaryUnits))
            .WithState(_ => "ALVSVAL108")
            .WithMessage(
                (p, c) =>
                    $"Supplementary units format on item number {c.ItemNumber} is invalid. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated. Enter it in the format 99999999999.999."
            );

        RuleForEach(p => p.NewClearanceRequest.Commodities)
            .Must(commodity => HaveAValidCommodityDecimalFormat(commodity.NetMass))
            .WithState(_ => "ALVSVAL109")
            .WithMessage(
                (p, c) =>
                    $"Net mass format on item number {c.ItemNumber} is invalid. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated. Enter it in the format 99999999999.999."
            );

        When(
            p => p.NewClearanceRequest.ExternalVersion != 1,
            () =>
            {
                RuleFor(p => p.NewClearanceRequest.PreviousExternalVersion)
                    .NotNull()
                    .WithState(_ => "ALVSVAL152")
                    .WithMessage(p =>
                        $"PreviousVersionNumber has not been provided for the import document. Provide a PreviousVersionNumber. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            }
        );

        RuleFor(p => p.NewClearanceRequest.ExternalVersion)
            .NotNull()
            .WithState(_ => "ALVSVAL153")
            .WithMessage(p =>
                $"EntryVersionNumber has not been provided for the import document. Provide an EntryVersionNumber. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            );

        RuleForEach(p =>
                (p.NewClearanceRequest.Commodities ?? Array.Empty<Commodity>()).GroupBy(c => c.ItemNumber).ToList()
            )
            .Must(grouping => grouping.Count() == 1)
            .OverridePropertyName("Commodities")
            .WithState(_ => "ALVSVAL164")
            .WithMessage(
                (p, grouping) =>
                    $"Item number {grouping.Key} is invalid as it appears more than once. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            );

        When(
            p => p.ExistingClearanceRequest is not null,
            () =>
            {
                RuleFor(p => p)
                    .Must(p => NotBeADuplicateEntryVersionNumber(p.NewClearanceRequest, p.ExistingClearanceRequest))
                    .WithState(_ => "ALVSVAL303")
                    .WithMessage(p =>
                        $"The import declaration was received as a new declaration. There is already a current import declaration in BTMS with EntryReference {p.Mrn} and EntryVersionNumber {p.ExistingClearanceRequest?.ExternalVersion}. Your request with Correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            }
        );

        RuleForEach(p =>
                WithSubject(
                    p,
                    OrEmpty(p.NewClearanceRequest.Commodities)
                        .Select(c =>
                            OrEmpty(c.Documents)
                                .Select(doc => new DocumentByCommodity { Commodity = c, Document = doc })
                        )
                        .SelectMany(x => x)
                )
            )
            .Must(c =>
                c.Subject.Document.DocumentCode != null && validDocumentCodes.Contains(c.Subject.Document.DocumentCode)
            )
            .OverridePropertyName("DocumentCode")
            .WithState(_ => "ALVSVAL308")
            .WithMessage(
                (_, c) =>
                    $"DocumentCode {c.Subject.Document.DocumentCode} on item number {c.Subject.Commodity.ItemNumber} is invalid. Your request with correlation ID {c.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            );

        RuleForEach(p => p.NewClearanceRequest.Commodities)
            .Must(commodity => (commodity.Checks ?? []).Any(check => check.CheckCode != null))
            .WithState(_ => "ALVSVAL311")
            .WithMessage(
                (p, c) =>
                    $"The CheckCode field on item number {c.ItemNumber} must have a value. Your service request with Correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewClearanceRequest.DeclarationUcr)
            .NotNull()
            .WithState(_ => "ALVSVAL313")
            .WithMessage(p =>
                $"The DeclarationUCR field must have a value.  Your service request with Correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => WithSubject(p, p.NewClearanceRequest.Commodities ?? Array.Empty<Commodity>()))
            .ForEach(p =>
            {
                p.Must(s =>
                    {
                        var hasMoreThanOneDepartmentCheck = (s.Subject.Checks ?? [])
                            .GroupBy(check => check.DepartmentCode)
                            .Any(g => g.Count() > 1);
                        return !hasMoreThanOneDepartmentCheck;
                    })
                    .WithState(_ => "ALVSVAL317")
                    .WithMessage(
                        (_, s) =>
                            $"Item {s.Subject.ItemNumber} has more than one Item Check defined for the same authority. You can only provide one. Your service request with Correlation ID {s.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            });

        RuleFor(p => WithSubject(p, OrEmpty(p.NewClearanceRequest.Commodities)))
            .ForEach(p =>
            {
                p.Must(s => s.Subject.Documents?.Length > 0)
                    .WithState(_ => "ALVSVAL318")
                    .WithMessage(
                        (_, s) =>
                            $"Item {s.Subject.ItemNumber} has no document code. BTMS requires at least one item document. Your request with correlation ID {s.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            });

        RuleFor(p =>
                WithSubject(
                    p,
                    OrEmpty(p.NewClearanceRequest.Commodities)
                        .Select(commodity =>
                            OrEmpty(commodity.Documents)
                                .Select(doc => new DocumentByCommodity { Commodity = commodity, Document = doc })
                        )
                        .SelectMany(x => x)
                )
            )
            .ForEach(p =>
            {
                p.Must(s =>
                    {
                        var relevantDocumentChecks = CommodityDocumentCheckMap.Where(map =>
                            map.DocumentCode == s.Subject.Document.DocumentCode
                        );
                        return OrEmpty(s.Subject.Commodity.Checks)
                            .All(check => relevantDocumentChecks.Any(map => map.CheckCode == check.CheckCode));
                    })
                    .WithState(_ => "ALVSVAL320")
                    .WithMessage(
                        (_, d) =>
                            $"Document code {d.Subject.Document.DocumentCode} is not appropriate for the check code requested on ItemNumber {d.Subject.Commodity.ItemNumber}. Your request with correlation ID {d.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            });

        RuleFor(p =>
                WithSubject(
                    p,
                    OrEmpty(p.NewClearanceRequest.Commodities)
                        .Select(commodity =>
                            OrEmpty(commodity.Checks)
                                .Select(check => new CheckByCommodity { Check = check, Commodity = commodity })
                        )
                        .SelectMany(x => x)
                )
            )
            .ForEach(p =>
            {
                p.Must(s => OrEmpty(s.Subject.Commodity.Documents).Any())
                    .WithState(_ => "ALVSVAL321")
                    .WithMessage(
                        (_, d) =>
                            $"Check code {d.Subject.Check.CheckCode} on ItemNumber {d.Subject.Commodity.ItemNumber} must have a document code. Your request with Correlation ID {d.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            });

        When(
            p => p.ExistingFinalisation is not null,
            () =>
            {
                RuleFor(p => p.ExistingFinalisation)
                    .Must(NotBeCancelled)
                    .WithState(_ => "ALVSVAL324")
                    .WithMessage(p =>
                        $"The Import Declaration with Entry Reference {p.Mrn} and EntryVersionNumber {p.NewClearanceRequest.ExternalVersion} was received but the Import Declaration was cancelled. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            }
        );

        RuleFor(p => p.NewClearanceRequest)
            .Must(p => p.PreviousExternalVersion < p.ExternalVersion)
            .OverridePropertyName("PreviousExternalVersion")
            .WithState(_ => "ALVSVAL326")
            .WithMessage(
                (_, p) =>
                    $"The previous version number {p.PreviousExternalVersion} on the entry document must be less than the entry version number. Your service request with Correlation ID {p.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => WithSubject(p, OrEmpty(p.NewClearanceRequest.Commodities)))
            .ForEach(p =>
            {
                p.Must(c =>
                    {
                        var checkCodes = (c.Subject.Checks ?? []).Select(check => check.CheckCode).ToList();
                        var hasIuuCheckCode = checkCodes.Contains("H224");
                        var hasPoaoCheckCode = checkCodes.Contains("H222");

                        return !hasIuuCheckCode || hasPoaoCheckCode;
                    })
                    .WithState(_ => "ALVSVAL328")
                    .WithMessage(
                        (_, cvc) =>
                            $"An IUU document has been specified for ItemNumber {cvc.Subject.ItemNumber}. Request a manual clearance if the item does not require a CHED P. Your request with correlation ID {cvc.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            });
    }

    private static bool NotBeADuplicateEntryVersionNumber(
        ClearanceRequest newClearanceRequest,
        ClearanceRequest? existingClearanceRequest
    )
    {
        return newClearanceRequest.ExternalVersion != existingClearanceRequest?.ExternalVersion;
    }

    private static bool NotBeCancelled(Finalisation? existingFinalisation)
    {
        return !existingFinalisation?.FinalState.IsCancelled() ?? true;
    }

    private static bool HaveAValidCommodityDecimalFormat(decimal? value)
    {
        var supplementaryUnits = value.ToString() ?? "";
        var length = supplementaryUnits.Replace(".", "").Length;
        var numDecimals = supplementaryUnits.SkipWhile(c => c != '.').Skip(1).Count();
        return length <= 14 && numDecimals <= 3;
    }
}
