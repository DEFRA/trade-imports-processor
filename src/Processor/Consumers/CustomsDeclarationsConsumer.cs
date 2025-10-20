using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Utils.CorrelationId;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation;
using FluentValidation.Results;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using DataApiErrors = Defra.TradeImportsDataApi.Domain.Errors;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class CustomsDeclarationsConsumer(
    ILogger<CustomsDeclarationsConsumer> logger,
    ITradeImportsDataApiClient api,
    IValidator<ClearanceRequestValidatorInput> clearanceRequestValidator,
    IValidator<CustomsDeclarationsMessage> customsDeclarationsMessageValidator,
    IValidator<DataApiCustomsDeclaration.ExternalError> errorNotificationValidator,
    IValidator<FinalisationValidatorInput> finalisationValidator,
    ICorrelationIdGenerator correlationIdGenerator
) : IConsumer<JsonElement>, IConsumerWithContext
{
    private const string InboundHmrcMessageTypeHeader = "InboundHmrcMessageType";
    private string MessageId => Context.GetTransportMessage().MessageId;

    private async Task ReportValidationErrors(
        CustomsDeclarationsMessage customsDeclarationsMessage,
        ValidationResult validationResult,
        string messageBody,
        string messageType,
        CancellationToken cancellationToken
    )
    {
        var mrn = customsDeclarationsMessage.Header.EntryReference;

        validationResult.Errors.ForEach(error =>
            logger.LogInformation(
                "Mrn {Mrn} Version {Version} Type {MessageType} failed validation with {ErrorCode}: {ErrorMessage}",
                mrn,
                customsDeclarationsMessage.Header.EntryVersionNumber,
                messageType,
                error.CustomState ?? error.ErrorCode,
                error.ErrorMessage
            )
        );

        var existingValidationErrors = await api.GetProcessingError(mrn, cancellationToken);

        var validationErrors = validationResult
            .Errors.Select(error => new DataApiErrors.ErrorItem
            {
                Code = (string)error.CustomState,
                Message = error.ErrorMessage,
            })
            .ToArray();

        if (validationErrors.Length == 0)
            return;

        var processingErrorNotification = new DataApiErrors.ProcessingError
        {
            Created = DateTime.UtcNow,
            CorrelationId = correlationIdGenerator.Generate(),
            SourceExternalCorrelationId = customsDeclarationsMessage.ServiceHeader.CorrelationId,
            ExternalVersion = customsDeclarationsMessage.Header.EntryVersionNumber,
            Errors = validationErrors,
            Message = messageBody,
        };

        var updatedValidationErrors = (existingValidationErrors?.ProcessingErrors ?? [])
            .Append(processingErrorNotification)
            .ToArray();

        await api.PutProcessingError(mrn, updatedValidationErrors, existingValidationErrors?.ETag, cancellationToken);
    }

    private static DataApiCustomsDeclaration.CustomsDeclaration UpdatedCustomsDeclaration<T>(
        CustomsDeclarationResponse? existingCustomsDeclaration,
        T updated
    )
    {
        return new DataApiCustomsDeclaration.CustomsDeclaration
        {
            ClearanceDecision = existingCustomsDeclaration?.ClearanceDecision,
            ClearanceRequest =
                updated as DataApiCustomsDeclaration.ClearanceRequest ?? existingCustomsDeclaration?.ClearanceRequest,
            Finalisation =
                updated as DataApiCustomsDeclaration.Finalisation ?? existingCustomsDeclaration?.Finalisation,
            ExternalErrors =
                updated as DataApiCustomsDeclaration.ExternalError[] ?? existingCustomsDeclaration?.ExternalErrors,
        };
    }

    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        var success = Context.Headers.TryGetValue(InboundHmrcMessageTypeHeader, out var inboundHmrcMessageTypeObject);
        var inboundHmrcMessageType = inboundHmrcMessageTypeObject?.ToString();
        var customsDeclarationsMessage = received.Deserialize<CustomsDeclarationsMessage>();

        if (!success || inboundHmrcMessageType == null || customsDeclarationsMessage == null)
            throw new CustomsDeclarationMessageTypeException(MessageId);

        var customsDeclarationsMessageValidation = await customsDeclarationsMessageValidator.ValidateAsync(
            customsDeclarationsMessage,
            cancellationToken
        );
        if (!customsDeclarationsMessageValidation.IsValid)
        {
            await ReportValidationErrors(
                customsDeclarationsMessage,
                customsDeclarationsMessageValidation,
                received.GetRawText(),
                inboundHmrcMessageType,
                cancellationToken
            );

            return;
        }

        var mrn = customsDeclarationsMessage.Header.EntryReference;
        var existingCustomsDeclaration = await api.GetCustomsDeclaration(mrn, cancellationToken);

        var (updatedCustomsDeclaration, validationError) = inboundHmrcMessageType switch
        {
            InboundHmrcMessageType.ClearanceRequest => await OnHandleClearanceRequest(
                mrn,
                received,
                existingCustomsDeclaration,
                cancellationToken
            ),
            InboundHmrcMessageType.InboundError => await OnHandleInboundError(
                mrn,
                received,
                existingCustomsDeclaration,
                cancellationToken
            ),
            InboundHmrcMessageType.Finalisation => await OnHandleFinalisation(
                mrn,
                received,
                existingCustomsDeclaration,
                cancellationToken
            ),
            _ => throw new CustomsDeclarationMessageTypeException(MessageId),
        };

        if (validationError != null)
        {
            await ReportValidationErrors(
                customsDeclarationsMessage,
                validationError,
                received.GetRawText(),
                inboundHmrcMessageType,
                cancellationToken
            );

            return;
        }

        if (updatedCustomsDeclaration == null)
            return;

        logger.LogInformation(
            "{Action} customs declaration for {Mrn} with Etag {Etag} and new state {State}",
            existingCustomsDeclaration != null ? "Updating" : "Creating",
            mrn,
            existingCustomsDeclaration?.ETag,
            updatedCustomsDeclaration.State()
        );

        await api.PutCustomsDeclaration(
            mrn,
            updatedCustomsDeclaration,
            existingCustomsDeclaration?.ETag,
            cancellationToken
        );
    }

    public required IConsumerContext Context { get; set; }

    private T DeserializeMessage<T>(JsonElement received, string mrn)
        where T : class
    {
        var result = received.Deserialize<T>();
        if (result == null)
            throw new CustomsDeclarationMessageException(MessageId);

        logger.LogInformation("Received {Type} for {Mrn}", typeof(T).Name, mrn);

        return result;
    }

    private async Task<(DataApiCustomsDeclaration.CustomsDeclaration?, ValidationResult?)> OnHandleClearanceRequest(
        string mrn,
        JsonElement received,
        CustomsDeclarationResponse? existingCustomsDeclaration,
        CancellationToken cancellationToken
    )
    {
        var unmappedClearanceRequest = DeserializeMessage<ClearanceRequest>(received, mrn);

        //This is here because the mapping uses an explicit operator and we can't add logic there
        LogAnyDocumentReferenceWithWhitespaceSuffix(unmappedClearanceRequest, logger);

        var mappedClearanceRequest = (DataApiCustomsDeclaration.ClearanceRequest)
            DeserializeMessage<ClearanceRequest>(received, mrn);

        var validationResult = await clearanceRequestValidator.ValidateAsync(
            new ClearanceRequestValidatorInput
            {
                Mrn = mrn,
                NewClearanceRequest = mappedClearanceRequest,
                ExistingClearanceRequest = existingCustomsDeclaration?.ClearanceRequest,
            },
            cancellationToken
        );

        if (!validationResult.IsValid)
        {
            return (null, validationResult);
        }

        if (
            existingCustomsDeclaration?.ClearanceRequest == null
            || IsClearanceRequestNewerThan(mappedClearanceRequest, existingCustomsDeclaration.ClearanceRequest)
        )
            return (UpdatedCustomsDeclaration(existingCustomsDeclaration, mappedClearanceRequest), null);

        logger.LogInformation(
            "Skipping {Mrn} because new {Type} {NewClearanceVersion} is older than existing {ExistingClearanceVersion}",
            mrn,
            nameof(ClearanceRequest),
            mappedClearanceRequest.ExternalVersion,
            existingCustomsDeclaration.ClearanceRequest.ExternalVersion
        );

        return (null, null);
    }

    public static void LogAnyDocumentReferenceWithWhitespaceSuffix(ClearanceRequest clearanceRequest, ILogger logger)
    {
        if (clearanceRequest.Items != null)
        {
            var documentReferencesWithSpaces = clearanceRequest.Items.Where(x => x.Documents is not null)
                .SelectMany(x => x.Documents!)
                .Where(x => x.DocumentReference is not null && char.IsWhiteSpace(x.DocumentReference[^1]))
                .Select(x => x.DocumentReference);

            foreach (var documentReference in documentReferencesWithSpaces)
            {
                logger.LogInformation("{DocumentReference} contains whitespace at the end", documentReference);
            }
        }
    }

    private async Task<(DataApiCustomsDeclaration.CustomsDeclaration?, ValidationResult?)> OnHandleInboundError(
        string mrn,
        JsonElement received,
        CustomsDeclarationResponse? existingCustomsDeclaration,
        CancellationToken cancellationToken
    )
    {
        var inboundErrorNotifications = (DataApiCustomsDeclaration.ExternalError)
            DeserializeMessage<InboundError>(received, mrn);

        var validationResult = await errorNotificationValidator.ValidateAsync(
            inboundErrorNotifications,
            cancellationToken
        );

        if (!validationResult.IsValid)
        {
            return (null, validationResult);
        }

        var updatedInboundError = (existingCustomsDeclaration?.ExternalErrors ?? [])
            .Append(inboundErrorNotifications)
            .ToArray();

        return (UpdatedCustomsDeclaration(existingCustomsDeclaration, updatedInboundError), null);
    }

    private async Task<(DataApiCustomsDeclaration.CustomsDeclaration?, ValidationResult?)> OnHandleFinalisation(
        string mrn,
        JsonElement received,
        CustomsDeclarationResponse? existingCustomsDeclaration,
        CancellationToken cancellationToken
    )
    {
        var finalisation = (DataApiCustomsDeclaration.Finalisation)DeserializeMessage<Finalisation>(received, mrn);

        if (existingCustomsDeclaration?.ClearanceRequest == null)
        {
            logger.LogInformation("Skipping finalisation of {Mrn} because no clearance request exists", mrn);

            return (null, null);
        }

        var validationResult = await finalisationValidator.ValidateAsync(
            new FinalisationValidatorInput
            {
                Mrn = mrn,
                NewFinalisation = finalisation,
                ExistingClearanceRequest = existingCustomsDeclaration.ClearanceRequest,
                ExistingFinalisation = existingCustomsDeclaration.Finalisation,
            },
            cancellationToken
        );

        if (!validationResult.IsValid)
        {
            return (null, validationResult);
        }

        if (
            existingCustomsDeclaration.Finalisation == null
            || IsFinalisationMessageNewerThan(finalisation, existingCustomsDeclaration.Finalisation)
        )
            return (UpdatedCustomsDeclaration(existingCustomsDeclaration, finalisation), null);

        logger.LogInformation(
            "Skipping {Mrn} because new {Type} {NewFinalisationSentAt:O} is older than existing {ExistingFinalisationSentAt:O}",
            mrn,
            nameof(Finalisation),
            finalisation.MessageSentAt,
            existingCustomsDeclaration.Finalisation.MessageSentAt
        );

        return (null, null);
    }

    private static bool IsClearanceRequestNewerThan(
        DataApiCustomsDeclaration.ClearanceRequest newClearanceRequest,
        DataApiCustomsDeclaration.ClearanceRequest existingClearanceRequest
    )
    {
        return newClearanceRequest.ExternalVersion > existingClearanceRequest.ExternalVersion;
    }

    private static bool IsFinalisationMessageNewerThan(
        DataApiCustomsDeclaration.Finalisation newFinalisation,
        DataApiCustomsDeclaration.Finalisation existingFinalisation
    )
    {
        return newFinalisation.ExternalVersion > existingFinalisation.ExternalVersion;
    }
}
