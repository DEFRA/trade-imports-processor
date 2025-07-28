using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Extensions;
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

        var gmrId = dataApiGmr.Id!;

        logger.LogInformation("Received Gmr for {GmrId}", gmrId);

        var existingGmr = await api.GetGmr(gmrId, cancellationToken);
        if (existingGmr != null && !ShouldProcess(dataApiGmr, existingGmr.Gmr))
        {
            return;
        }

        if (existingGmr == null)
        {
            await api.PutGmr(gmrId, dataApiGmr, null, cancellationToken);

            return;
        }

        logger.LogInformation("Updating existing Gmr {GmrId}", gmrId);

        await api.PutGmr(gmrId, dataApiGmr, existingGmr.ETag, cancellationToken);
    }

    private bool ShouldProcess(DataApiGvms.Gmr newGmr, DataApiGvms.Gmr existingGmr)
    {
        if (NewGmrIsOlderThanExistingGmr(newGmr, existingGmr))
        {
            logger.LogInformation(
                "Skipping {GmrId} because new Gmr is older: {NewTime:O} < {OldTime:O}",
                newGmr.Id,
                newGmr.UpdatedSource,
                existingGmr.UpdatedSource
            );

            return false;
        }

        return true;
    }

    private static bool NewGmrIsOlderThanExistingGmr(DataApiGvms.Gmr newGmr, DataApiGvms.Gmr existingGmr)
    {
        return newGmr.UpdatedSource.TrimMicroseconds() <= existingGmr.UpdatedSource.TrimMicroseconds();
    }
}
