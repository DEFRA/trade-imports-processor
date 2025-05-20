using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using FluentValidation;
using FluentValidation.Results;
using SlimMessageBus;
using SlimMessageBus.Host.AzureServiceBus;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class GmrsConsumer(
    ILogger<GmrsConsumer> logger,
    ITradeImportsDataApiClient api,
    IValidator<DataApiGvms.Gmr> validator
) : IConsumer<JsonElement>, IConsumerWithContext
{
    private string MessageId => Context.GetTransportMessage().MessageId;
    public required IConsumerContext Context { get; set; }

    private void LogValidationErrors(Gmr gmr, ValidationResult validationResult)
    {
        validationResult.Errors.ForEach(error =>
            logger.LogInformation(
                "Gmr {GmrId} failed validation with {ErrorCode}: {ErrorMessage}",
                gmr.GmrId,
                error.CustomState ?? error.ErrorCode,
                error.ErrorMessage
            )
        );
    }

    public async Task OnHandle(JsonElement message, CancellationToken cancellationToken)
    {
        var gmr = message.Deserialize<Gmr>();
        if (gmr == null)
        {
            throw new GmrMessageException(MessageId);
        }

        var dataApiGmr = (DataApiGvms.Gmr)gmr;
        var validationResult = await validator.ValidateAsync(dataApiGmr, cancellationToken);
        if (!validationResult.IsValid)
        {
            LogValidationErrors(gmr, validationResult);
            return;
        }

        logger.LogInformation("Received Gmr for {GmrId}", gmr.GmrId);

        await api.PutGmr(dataApiGmr.Id!, dataApiGmr, null, cancellationToken);
    }
}
