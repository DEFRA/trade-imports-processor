using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Extensions;
using FluentValidation;
using FluentValidation.Results;
using SlimMessageBus;
using Trade.Gateway.Api.Contract.Certificate;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class TracesChedConsumer(
    ILogger<TracesChedConsumer> logger,
    ITradeImportsDataApiClient api,
    IValidator<DefraUNVTDCHEDProfile> validator
) : IConsumer<DefraUNVTDCHEDProfile>
{
    public async Task OnHandle(DefraUNVTDCHEDProfile received, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(received, cancellationToken);
        if (!validationResult.IsValid)
        {
            LogValidationErrors(received, validationResult);
            return;
        }

        var chedReference = received.ExchangedDocument.Identifier;

        logger.LogInformation("Received Traces Ched {ReferenceNumber}", chedReference);

        var existingChed = await api.GetTracesChed(chedReference, cancellationToken);

        if (existingChed == null)
        {
            await CreateChed(received, cancellationToken);

            return;
        }

        if (ShouldProcess(received, existingChed.Ched))
        {
            await UpdateChed(existingChed.ETag!, received, cancellationToken);
        }
    }

    private void LogValidationErrors(DefraUNVTDCHEDProfile ched, ValidationResult validationResult)
    {
        validationResult.Errors.ForEach(error =>
            logger.LogInformation(
                "Traces Ched {Id} failed validation with {ErrorCode}: {ErrorMessage}",
                ched.ExchangedDocument.Identifier,
                error.CustomState ?? error.ErrorCode,
                error.ErrorMessage
            )
        );
    }

    private async Task UpdateChed(string eTag, DefraUNVTDCHEDProfile ched, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating existing Traces Ched {ReferenceNumber}, status {Status}, updated source {UpdatedSource:O}",
            ched.ExchangedDocument.Identifier,
            ched.ExchangedDocument.NotificationStatusCode,
            ched.GetLatestLastUpdateDateTime()
        );

        await api.PutTracesChed(ched.ExchangedDocument.Identifier, ched, eTag, cancellationToken);
    }

    private async Task CreateChed(DefraUNVTDCHEDProfile ched, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating new Traces Ched {ReferenceNumber}, status {Status}, updated source {UpdatedSource:O}",
            ched.ExchangedDocument.Identifier,
            ched.ExchangedDocument.NotificationStatusCode,
            ched.GetLatestLastUpdateDateTime()
        );

        await api.PutTracesChed(ched.ExchangedDocument.Identifier, ched, null, cancellationToken);
    }

    private bool ShouldProcess(DefraUNVTDCHEDProfile newChed, DefraUNVTDCHEDProfile existingChed)
    {
        var newChedTime = newChed.GetLatestLastUpdateDateTime();
        var existingChedTime = existingChed.GetLatestLastUpdateDateTime();
        if (newChedTime > existingChedTime)
        {
            return true;
        }

        logger.LogInformation(
            "Skipping {ReferenceNumber} as timestamp on latest message received {NewTime:O} is before the timestamp {OldTime:O} on the current message.  This message appears to have been sent out of sequence.",
            newChed.ExchangedDocument.Identifier,
            newChedTime,
            existingChedTime
        );

        return false;
    }
}
