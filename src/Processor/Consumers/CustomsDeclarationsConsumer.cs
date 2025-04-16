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
    private string MessageId => Context.GetTransportMessage().MessageId;

    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        var success = Context.Headers.TryGetValue("InboundHmrcMessageType", out var inboundHmrcMessageType);
        if (!success || inboundHmrcMessageType == null)
            throw new CustomsDeclarationMessageTypeException(MessageId);

        var customsDeclarationsMessage = received.Deserialize<CustomsDeclarationsMessage>();
        if (customsDeclarationsMessage == null)
            throw new CustomsDeclarationMessageException(MessageId);
        var mrn = customsDeclarationsMessage.Header.EntryReference!;

        var existingCustomsDeclaration = await api.GetCustomsDeclaration(mrn, cancellationToken);

        var customsDeclaration = inboundHmrcMessageType switch
        {
            InboundHmrcMessageType.ClearanceRequest => OnHandleClearanceRequest(
                mrn,
                received,
                existingCustomsDeclaration
            ),
            _ => throw new CustomsDeclarationMessageTypeException(MessageId),
        };

        if (customsDeclaration == null)
            return;

        logger.LogInformation("Updating existing customs declaration for {Mrn}", mrn);
        await api.PutCustomsDeclaration(mrn, customsDeclaration, existingCustomsDeclaration?.ETag, cancellationToken);
    }

    public required IConsumerContext Context { get; set; }

    private DataApiCustomsDeclaration.CustomsDeclaration? OnHandleClearanceRequest(
        string mrn,
        JsonElement received,
        CustomsDeclarationResponse? existingCustomsDeclaration
    )
    {
        var clearanceRequest = received.Deserialize<ClearanceRequest>();
        if (clearanceRequest == null)
            throw new CustomsDeclarationMessageException(MessageId);

        logger.LogInformation("Received clearance request for {MessageId}", MessageId);

        var newClearanceRequest = (DataApiCustomsDeclaration.ClearanceRequest)clearanceRequest;

        if (existingCustomsDeclaration == null)
            return new DataApiCustomsDeclaration.CustomsDeclaration { ClearanceRequest = newClearanceRequest };

        if (
            existingCustomsDeclaration.ClearanceRequest == null
            || IsClearanceRequestNewerThan(newClearanceRequest, existingCustomsDeclaration.ClearanceRequest)
        )
            return new DataApiCustomsDeclaration.CustomsDeclaration
            {
                ClearanceDecision = existingCustomsDeclaration.ClearanceDecision,
                ClearanceRequest = newClearanceRequest,
                Finalisation = existingCustomsDeclaration.Finalisation,
            };

        logger.LogInformation(
            "Skipping {Mrn} because new clearance request {NewClearanceVersion} is older than existing {ExistingClearanceVersion}",
            mrn,
            newClearanceRequest.ExternalVersion,
            existingCustomsDeclaration.ClearanceRequest.ExternalVersion
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
}
