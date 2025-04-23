using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using FluentValidation;
using ClearanceRequest = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.ClearanceRequest;
using Finalisation = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class ClearanceRequestValidator : AbstractValidator<ClearanceRequestValidatorInput>
{
    public readonly HashSet<string> ValidDocumentCodes =
    [
        "C633",
        "C640",
        "C641",
        "C673",
        "N002",
        "N851",
        "N852",
        "C678",
        "N853",
        "9HCG",
        "9115",
        "C085",
    ];

    public ClearanceRequestValidator()
    {
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
                (p.NewClearanceRequest.Commodities ?? Array.Empty<Commodity>())
                    .Select(c =>
                        (c.Documents ?? Array.Empty<ImportDocument>())
                            .Where(doc => p.NewClearanceRequest.ExternalCorrelationId != null && c.ItemNumber != null)
                            .Select(doc => new CommodityImportDocument
                            {
                                Document = doc,
                                ExternalCorrelationId = p.NewClearanceRequest.ExternalCorrelationId!,
                                ItemNumber = (int)c.ItemNumber!,
                            })
                    )
                    .SelectMany(c => c)
            )
            .Must(c => c.Document.DocumentCode != null && ValidDocumentCodes.Contains(c.Document.DocumentCode))
            .OverridePropertyName("CommoditiesDocumentCode")
            .WithState(_ => "ALVSVAL308")
            .WithMessage(
                (p, c) =>
                    $"DocumentCode {c.Document.DocumentCode} on item number {c.ItemNumber} is invalid. Your request with correlation ID {c.ExternalCorrelationId} has been terminated."
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
    }

    private sealed record CommodityImportDocument
    {
        public required string ExternalCorrelationId { get; init; }
        public required int ItemNumber { get; init; }
        public required ImportDocument Document { get; init; }
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
