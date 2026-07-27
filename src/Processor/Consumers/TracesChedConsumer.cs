using System.Globalization;
using System.Text.Json;
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
        var newChed = received;

        if (newChed == null)
        {
            throw new InvalidOperationException("Received invalid message, deserialised as null");
        }

        var chedReference = newChed.ExchangedDocument.Identifier;

        logger.LogInformation("Received Traces Ched {ReferenceNumber}", chedReference);

        var existingChed = await api.GetTracesChed(chedReference, cancellationToken);

        if (existingChed == null)
        {
            await CreateChed(newChed, cancellationToken);

            return;
        }

        if (ShouldProcess(newChed, existingChed.Ched))
        {
            await UpdateChed(existingChed.ETag!, newChed, cancellationToken);
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

    private static bool ShouldProcess(DefraUNVTDCHEDProfile newChed, DefraUNVTDCHEDProfile existingChed)
    {
        return GetLatestLastUpdateDateTime(newChed) > GetLatestLastUpdateDateTime(existingChed);
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
