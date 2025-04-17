using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class CustomsDeclarationsConsumer(ILogger<CustomsDeclarationsConsumer> logger, ITradeImportsDataApiClient api)
    : IConsumer<JsonElement>,
        IConsumerWithContext
{
    private const string InboundHmrcMessageTypeHeader = "InboundHmrcMessageType";
    private string MessageId => Context.GetTransportMessage().MessageId;

    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        var success = Context.Headers.TryGetValue(InboundHmrcMessageTypeHeader, out var inboundHmrcMessageType);
        var customsDeclarationsMessage = received.Deserialize<CustomsDeclarationsMessage>();

        if (!success || inboundHmrcMessageType == null || customsDeclarationsMessage == null)
            throw new CustomsDeclarationMessageTypeException(MessageId);

        var mrn = customsDeclarationsMessage.Header.EntryReference;
        var existingCustomsDeclaration = await api.GetCustomsDeclaration(mrn, cancellationToken);

        var updatedCustomsDeclaration = inboundHmrcMessageType switch
        {
            InboundHmrcMessageType.ClearanceRequest => OnHandleClearanceRequest(
                mrn,
                received,
                existingCustomsDeclaration
            ),
            InboundHmrcMessageType.InboundError => null,
            InboundHmrcMessageType.Finalisation => OnHandleFinalisation(mrn, received, existingCustomsDeclaration),
            _ => throw new CustomsDeclarationMessageTypeException(MessageId),
        };

        if (updatedCustomsDeclaration == null)
            return;

        logger.LogInformation(
            "{Action} customs declaration for {Mrn}",
            existingCustomsDeclaration != null ? "Updating" : "Creating",
            mrn
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

    private DataApiCustomsDeclaration.CustomsDeclaration? OnHandleClearanceRequest(
        string mrn,
        JsonElement received,
        CustomsDeclarationResponse? existingCustomsDeclaration
    )
    {
        var clearanceRequest = (DataApiCustomsDeclaration.ClearanceRequest)
            DeserializeMessage<ClearanceRequest>(received, mrn);

        if (
            existingCustomsDeclaration?.ClearanceRequest == null
            || IsClearanceRequestNewerThan(clearanceRequest, existingCustomsDeclaration.ClearanceRequest)
        )
            return new DataApiCustomsDeclaration.CustomsDeclaration
            {
                ClearanceDecision = existingCustomsDeclaration?.ClearanceDecision,
                ClearanceRequest = clearanceRequest,
                Finalisation = existingCustomsDeclaration?.Finalisation,
            };

        logger.LogInformation(
            "Skipping {Mrn} because new {Type} {NewClearanceVersion} is older than existing {ExistingClearanceVersion}",
            mrn,
            nameof(ClearanceRequest),
            clearanceRequest.ExternalVersion,
            existingCustomsDeclaration.ClearanceRequest.ExternalVersion
        );

        return null;
    }

    private DataApiCustomsDeclaration.CustomsDeclaration? OnHandleFinalisation(
        string mrn,
        JsonElement received,
        CustomsDeclarationResponse? existingCustomsDeclaration
    )
    {
        var finalisation = (DataApiCustomsDeclaration.Finalisation)DeserializeMessage<Finalisation>(received, mrn);

        if (
            existingCustomsDeclaration?.Finalisation == null
            || IsFinalisationMessageNewerThan(finalisation, existingCustomsDeclaration.Finalisation)
        )
            return new DataApiCustomsDeclaration.CustomsDeclaration
            {
                ClearanceDecision = existingCustomsDeclaration?.ClearanceDecision,
                ClearanceRequest = existingCustomsDeclaration?.ClearanceRequest,
                Finalisation = finalisation,
            };

        logger.LogInformation(
            "Skipping {Mrn} because new {Type} {NewFinalisationSentAt} is older than existing {ExistingFinalisationSentAt}",
            mrn,
            nameof(Finalisation),
            finalisation.MessageSentAt,
            existingCustomsDeclaration.Finalisation.MessageSentAt
        );

        return null;
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
