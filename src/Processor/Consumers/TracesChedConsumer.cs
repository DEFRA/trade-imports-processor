using System.Globalization;
using Defra.TradeImportsDataApi.Api.Client;
using SlimMessageBus;
using Trade.Gateway.Api.Contract.Certificate;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class TracesChedConsumer(ILogger<TracesChedConsumer> logger, ITradeImportsDataApiClient api)
    : IConsumer<DefraUNVTDCHEDProfile>
{
    private const string LastUpdateSubjectCode = "LAST_UPDATE_DATETIME";

    public async Task OnHandle(DefraUNVTDCHEDProfile received, CancellationToken cancellationToken)
    {
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

    private async Task UpdateChed(string eTag, DefraUNVTDCHEDProfile ched, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating existing Traces Ched {ReferenceNumber}, status {Status}, updated source {UpdatedSource:O}",
            ched.ExchangedDocument.Identifier,
            ched.ExchangedDocument.NotificationStatusCode,
            GetLatestLastUpdateDateTime(ched)
        );

        await api.PutTracesChed(ched.ExchangedDocument.Identifier, ched, eTag, cancellationToken);
    }

    private async Task CreateChed(DefraUNVTDCHEDProfile ched, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating new Traces Ched {ReferenceNumber}, status {Status}, updated source {UpdatedSource:O}",
            ched.ExchangedDocument.Identifier,
            ched.ExchangedDocument.NotificationStatusCode,
            GetLatestLastUpdateDateTime(ched)
        );

        await api.PutTracesChed(ched.ExchangedDocument.Identifier, ched, null, cancellationToken);
    }

    private bool ShouldProcess(DefraUNVTDCHEDProfile newChed, DefraUNVTDCHEDProfile existingChed)
    {
        var newChedTime = GetLatestLastUpdateDateTime(newChed);
        var existingChedTime = GetLatestLastUpdateDateTime(existingChed);
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

    private static DateTimeOffset? GetLatestLastUpdateDateTime(DefraUNVTDCHEDProfile ched)
    {
        var notes = ched.ExchangedDocument.IncludedNote;
        if (notes == null)
        {
            return null;
        }

        return notes
            .Where(n => string.Equals(n.SubjectCode?.Value, LastUpdateSubjectCode, StringComparison.OrdinalIgnoreCase))
            .SelectMany(n => n.Content ?? Enumerable.Empty<string>())
            .Select(c =>
            {
                if (DateTimeOffset.TryParse(c, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
                {
                    return (DateTimeOffset?)dto;
                }

                return null;
            })
            .Where(d => d.HasValue)
            .Max();
    }
}
